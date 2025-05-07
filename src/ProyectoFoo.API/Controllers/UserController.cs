using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProyectoFoo.Application.Features.Users;
using Microsoft.AspNetCore.Authorization;// Necesario para StatusCodes y Authorize
using System.Security.Claims;
using ProyectoFoo.Shared;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.ServiceExtension;


namespace ProyectoFoo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger, IUserService userService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="command">Objeto JSON que representa la información del nuevo usuario.</param>
        /// <returns>Respuesta HTTP indicando el éxito de la creación. En caso de éxito, devuelve la información del usuario creado.</returns>
        /// <response code="201">Usuario creado exitosamente.</response>
        /// <response code="400">Error de validación o usuario ya existente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("register")]
        public async Task<ActionResult<CreateUserResponse>> CreateUser([FromBody] CreateUserCommand command)
        {
            System.Diagnostics.Debug.WriteLine($"Received CreateUserCommand: {System.Text.Json.JsonSerializer.Serialize(command)}");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Devuelve errores de validación
            }

            try
            {
                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    // return CreatedAtAction(nameof(GetUserById), new { id = response.User?.Id }, response.User);
                    return StatusCode(StatusCodes.Status201Created, response.User); // Devuelve 201 con el usuario creado response.User);
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

        [HttpPut("me")] // Usamos "me" para indicar que el usuario actual actualiza su propio perfil
        [Authorize] // Requiere que el usuario esté autenticado
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserDto updateUser)
        {
            // 1. Obtener el ID del usuario autenticado desde el token JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                _logger.LogWarning("No se pudo obtener el ID del usuario desde el token.");
                return Unauthorized(); // Si no se puede obtener el ID del usuario, la autenticación falló
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // 2. Llamar al servicio para actualizar el usuario
                var updatedUser = await _userService.UpdateUserAsync(currentUserId, updateUser);

                if (updatedUser == null)
                {
                    string message = $"No se encontró el usuario con ID {currentUserId} para actualizar.";
                    _logger.LogWarning(message);
                    return NotFound(); // El usuario con ese ID no se encontró
                }

                // 3. Devolver una respuesta exitosa con los datos actualizados
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                string message = $"Error al actualizar el usuario con ID {currentUserId}: {ex}";
                _logger.LogError(message);
                return StatusCode(500, "Ocurrió un error al actualizar el perfil. Inténtalo de nuevo más tarde.");
            }
        }

        /*
        [HttpPut("me/password")] //Endpoint para actualizar únicamente la contraseña 
        [Authorize]
        public async Task<IActionResult> UpdateCurrentUserPassword([FromBody] UpdatePasswordDto updatePassword)
        {
            // 1. Obtener el ID del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2. Llamar al servicio para actualizar la contraseña
            var result = await _userService.UpdateUserPasswordAsync(currentUserId, updatePassword);

            if (result is BadRequestObjectResult badRequestResult)
            {
                return badRequestResult; // Devuelve errores específicos (ej., contraseña actual incorrecta)
            }
            else if (result is StatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 500)
            {
                return StatusCode(500, "Ocurrió un error al actualizar la contraseña. Inténtalo de nuevo más tarde.");
            }
            else
            {
                return NoContent(); // Contraseña actualizada exitosamente
            }
        }*/

     }
}
