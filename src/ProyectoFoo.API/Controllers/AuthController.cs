using Microsoft.AspNetCore.Mvc;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Services;
using ProyectoFoo.API.Models.Authentication;

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

        public AuthController(IUserRepository usuarioRepository, ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(model.Email);

            if (usuario == null || !usuario.VerificarContrasena(model.Password))
            {
                return Unauthorized("Credenciales inválidas.");
            }

            // Generar el token utilizando el TokenService
            var token = _tokenService.GenerateToken(usuario);

            // Actualizar la fecha del último acceso
            usuario.ActualizarUltimoAcceso();
            await _usuarioRepository.UpdateAsync(usuario);

            return Ok(new { Token = token });
        }       
    }    
}