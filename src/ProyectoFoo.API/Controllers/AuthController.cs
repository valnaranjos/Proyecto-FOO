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
    /// Autentica a un usuario y devuelve un token JWT si las credenciales son válidas.
    /// </summary>
    /// <param name="model">Objeto JSON que contiene el correo electrónico y la contraseña del usuario.</param>
    /// <returns>
    /// Si las credenciales son correctas, devuelve un código de estado 200 OK con el token JWT.
    /// Si las credenciales son incorrectas, devuelve un código de estado 401 Unauthorized.
    /// </returns>
    /// <response code="200">Credenciales válidas. Devuelve el token JWT.</response>
    /// <response code="401">Credenciales inválidas.</response>
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
        /// <returns>
        /// Si las credenciales son correctas, devuelve un código de estado 200 OK con el token JWT.
        /// Si las credenciales son incorrectas, devuelve un código de estado 401 Unauthorized.
        /// </returns>
        /// <response code="200">Credenciales válidas. Devuelve el token JWT.</response>
        /// <response code="401">Credenciales inválidas.</response>
        [HttpPost("login")]
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
        /// Solicita el restablecimiento de contraseña para un usuario.
        /// </summary>
        /// <param name="command">Objeto que contiene el email del usuario.</param>
        /// <returns>Mensaje indicando si la solicitud fue procesada correctamente.</returns>
        /// <response code="200">La solicitud de restablecimiento fue enviada exitosamente.</response>
        /// <response code="400">El modelo es inválido o hubo un error al procesar la solicitud.</response>
        [HttpPost("request-password-reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);
            if (result == null || !result.Success)
            {
                return BadRequest("No se pudo restablecer la contraseña. Verifique el token y el email.");
            }

            return Ok(result.Message);
        }


        /// <summary>
        /// Verifica el código de restablecimiento de contraseña y actualiza la contraseña del usuario.
        /// </summary>
        /// <param name="command">Objeto que contiene el código, email y nueva contraseña.</param>
        /// <returns>Mensaje indicando el resultado del restablecimiento.</returns>
        /// <response code="200">Contraseña restablecida exitosamente.</response>
        /// <response code="400">Datos inválidos o error al restablecer la contraseña.</response>
        [HttpPost("verify-password-reset-code")]
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
        /// Endpoint para cerrar la sesión del usuario.
        /// </summary>
        /// <returns>Respuesta HTTP indicando el éxito del cierre de sesión.</returns>
        [Authorize] // Requiere que el usuario esté autenticado para acceder a este endpoint
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // La responsabilidad de dejar de usar el token recae en el frontend.

            return Ok(new { message = "Sesión cerrada exitosamente." });
        }
    }    
}