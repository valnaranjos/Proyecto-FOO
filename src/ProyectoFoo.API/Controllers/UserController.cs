using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProyectoFoo.Application.Features.Users;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Services;
using ProyectoFoo.API.Models.Authentication;
using ProyectoFoo.Shared.Models.User;
using ProyectoFoo.Application.Features.Users.CRUD;


namespace ProyectoFoo.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de usuarios.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IMediator mediator, ILogger<UserController> logger, IUserService userService, IEmailService emailService, ITokenService tokenService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        private readonly ILogger<UserController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));       
        private readonly ITokenService _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="command">Objeto JSON que representa la información del nuevo usuario.</param>
        /// <returns>Respuesta HTTP indicando el éxito de la creación. En caso de éxito, devuelve la información del usuario creado.</returns>
        /// <response code="201">Usuario creado exitosamente.</response>
        /// <response code="400">Error de validación o usuario ya existente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        
        public async Task<ActionResult<CreateUserResponse>> CreateUser([FromBody] CreateUserCommand command)
        {
            System.Diagnostics.Debug.WriteLine($"Received CreateUserCommand: {System.Text.Json.JsonSerializer.Serialize(command)}");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _mediator.Send(command);

                if (response.Success)
                {

                    return StatusCode(StatusCodes.Status201Created, response.User);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error al registrar el usuario. Inténtalo de nuevo más tarde.");
            }
        }

        /// <summary>
        /// Verifica el registro de un usuario mediante un código de verificación.
        /// </summary>
        /// <param name="command">Correo y código.</param>
        /// <returns>Token JWT si es exitoso.</returns>
        /// <response code="200">Verificación exitosa.</response>
        /// <response code="400">Código incorrecto o expirado.</response>

        [HttpPost("verify-registration")]
        [ProducesResponseType(typeof(VerifyRegistrationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyRegistration([FromBody] VerifyRegistrationCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Success)
            {
                return Ok(new { response.Message, response.Token });
            }
            return BadRequest(response.Message);
        }


        /// <summary>
        /// Obtiene la información del usuario autenticado actualmente.
        /// </summary>
        /// <returns>Datos del usuario actual.</returns>
        /// <response code="200">Datos obtenidos exitosamente.</response>
        /// <response code="401">Token inválido o no proporcionado.</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var query = new GetCurrentUserCommand(int.Parse(userIdClaim));
            var result = await _mediator.Send(query);

            return Ok(result);
        }



        /// <summary>
        /// Actualiza el perfil del usuario autenticado actual.
        /// </summary>
        /// <param name="updateUser">Datos a actualizar.</param>
        /// <returns>Usuario actualizado.</returns>
        /// <response code="200">Actualización exitosa.</response>
        /// <response code="400">Datos inválidos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="404">Usuario no encontrado.</response>
        /// <response code="500">Error interno.</response>

        [HttpPut("me/edit")] // Usamos "me" para indicar que el usuario actual actualiza su propio perfil
        [Authorize] // Requiere que el usuario esté autenticado
        [ProducesResponseType(typeof(UpdateUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserDto updateUser)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                _logger.LogWarning("No se pudo obtener el ID del usuario desde el token.");
                return Unauthorized(); 
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedUser = await _userService.UpdateUserAsync(currentUserId, updateUser);

                if (updatedUser == null)
                {
                    
                    return NotFound();
                }

                return Ok(updatedUser);
            }
            catch (Exception)
            {              
                return StatusCode(500, "Ocurrió un error al actualizar el perfil. Inténtalo de nuevo más tarde.");
            }
        }

        /// <summary>
        /// Cambia la contraseña del usuario autenticado.
        /// </summary>
        /// <param name="updatePassword">Contraseña actual y nueva.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="200">Contraseña actualizada.</response>
        /// <response code="400">Contraseña actual incorrecta.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="500">Error interno.</response>
        [HttpPut("me/password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCurrentUserPassword([FromBody] UpdatePasswordDto updatePassword)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userService.UpdateUserPasswordAsync(currentUserId, updatePassword);

                if (result)
                {
                    return StatusCode(200, "Cambiaste tu contraseña existosamente");
                }
                else
                {
                    return BadRequest("Error al cambiar la contraseña. Verifica tu contraseña actual.");
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error al cambiar la contraseña. Inténtalo de nuevo más tarde.");
            }
        }

        /// <summary>
        /// Solicita un cambio de correo electrónico.
        /// </summary>
        /// <param name="changeEmailRequest">Nuevo correo electrónico.</param>
        /// <returns>Resultado de la solicitud.</returns>
        /// <response code="200">Código enviado al nuevo correo.</response>
        /// <response code="400">Correo inválido o ya en uso.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="500">Error interno.</response>
        [HttpPut("me/change-email")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RequestEmailChange([FromBody] ChangeEmailRequestDto changeEmailRequest)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = await _userService.RequestEmailChangeAsync(currentUserId, changeEmailRequest.NewEmail);

                if (success)
                {
                    return Ok("Se ha enviado un código de verificación a tu nueva dirección de correo electrónico.");
                }
                else
                {
                    return BadRequest("Error al solicitar el cambio de correo electrónico. Inténtalo de nuevo más tarde.");
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error al solicitar el cambio de correo electrónico.");
            }
        }

        /// <summary>
        /// Confirma el cambio de correo electrónico.
        /// </summary>
        /// <param name="confirmEmailChange">Código de verificación y nuevo correo.</param>
        /// <returns>Resultado de la confirmación.</returns>
        /// <response code="200">Correo actualizado.</response>
        /// <response code="400">Código incorrecto o expirado.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="500">Error interno.</response>
        [HttpPost("me/confirm-email-change")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeDto confirmEmailChange)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = await _userService.ConfirmEmailChangeAsync(currentUserId, confirmEmailChange.NewEmail, confirmEmailChange.VerificationCode);

                if (success)
                {
                    return Ok("Tu dirección de correo electrónico ha sido actualizada exitosamente.");
                }
                else
                {
                    return BadRequest("El código de verificación es inválido o ha expirado. Inténtalo de nuevo.");
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error al confirmar el cambio de correo electrónico.");
            }
        }
    }
}
