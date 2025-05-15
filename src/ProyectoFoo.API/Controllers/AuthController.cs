using Microsoft.AspNetCore.Mvc;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Services;
using ProyectoFoo.API.Models.Authentication;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using ProyectoFoo.Application.Features.Login;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoFoo.API.Controllers
{
    /// <summary>
    /// Controlador responsable de gestionar la autenticación y recuperación de contraseñas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly IMediator _mediator;

        public AuthController(IUserRepository usuarioRepository, ITokenService tokenService, IMediator mediator)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Autentica a un usuario y devuelve un token JWT si las credenciales son válidas.
        /// </summary>
        /// <param name="model">Objeto JSON que contiene el correo electrónico y la contraseña del usuario.</param>
        /// <returns>Token JWT en caso de autenticación exitosa.</returns>
        /// <response code="200">Autenticación exitosa. Se devuelve el token JWT.</response>
        /// <response code="401">Credenciales inválidas o cuenta no verificada.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(model.Email);

            if (usuario == null || !usuario.VerifyPassword(model.Password))
            {
                return Unauthorized("Credenciales inválidas.");
            }

            if (!usuario.IsVerified)
            {
                return Unauthorized("Cuenta no verificada. Por favor, verifica tu correo electrónico.");
            }

            // Generar el token utilizando el TokenService
            var token = _tokenService.GenerateToken(usuario);

            // Actualizar la fecha del último acceso
            usuario.ActualizeLastAcces();
            await _usuarioRepository.UpdateAsync(usuario);

            return Ok(new { Token = token });
        }


        /// <summary>
        /// Inicia el proceso de restablecimiento de contraseña para un usuario registrado.
        /// </summary>
        /// <param name="command">Comando que contiene el email del usuario.</param>
        /// <returns>Mensaje de confirmación del envío del proceso de restablecimiento.</returns>
        /// <response code="200">Proceso de restablecimiento iniciado exitosamente.</response>
        /// <response code="400">Datos inválidos o error en el procesamiento.</response>
        [HttpPost("request-password-reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);
            if (result == null || !result.Success)
            {
                return BadRequest("No se pudo iniciar el proceso de restablecimiento de contraseña. Verifique el email ingresado."); ;
            }

            return Ok(result.Message);
        }


        /// <summary>
        /// Verifica el código de restablecimiento de contraseña y actualiza la contraseña del usuario.
        /// </summary>
        /// <param name="command">Comando con el código de verificación, correo y nueva contraseña.</param>
        /// <returns>Mensaje indicando el resultado del restablecimiento.</returns>
        /// <response code="200">Contraseña restablecida exitosamente.</response>
        /// <response code="400">Código inválido o error al restablecer la contraseña.</response>
        [HttpPost("verify-password-reset-code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyPasswordResetCode([FromBody] VerifyPasswordResetCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _mediator.Send(command);
            if (!response.Success)
            {
                return BadRequest("No se pudo restablecer la contraseña. Verifique el token y el email.");
            }

            return Ok("Contraseña restablecida exitosamente.");
        }

        /// <summary>
        /// Finaliza la sesión del usuario autenticado.
        /// </summary>
        /// <remarks>Este endpoint no invalida el token; el manejo debe hacerse desde el frontend.</remarks>
        /// <returns>Mensaje de cierre de sesión exitoso.</returns>
        /// <response code="200">Sesión cerrada exitosamente.</response>
        [Authorize] // Requiere que el usuario esté autenticado para acceder a este endpoint
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            // La responsabilidad de dejar de usar el token recae en el frontend.

            return Ok(new { message = "Sesión cerrada exitosamente." });
        }
    }    
}