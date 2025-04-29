using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProyectoFoo.Application.Contracts.Persistence;
using Microsoft.Extensions.Configuration;
using ProyectoFoo.Shared.Models;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.API.Models.Authentication;

namespace ProyectoFoo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IUserRepository usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var usuario = await _usuarioRepository.GetUsuarioPorEmail(model.Email);

            if (usuario != null && usuario.VerificarContrasena(model.Password))
            {
                usuario.ActualizarUltimoAcceso();
                await _usuarioRepository.UpdateUsuario(usuario); // Actualizar la fecha del último acceso

                var token = GenerateJwtToken(usuario);
                return Ok(new { Token = token });
            }

            return Unauthorized("Credenciales inválidas.");
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Name),
                new Claim(ClaimTypes.Email, usuario.Email),
                // Aquí se pueden agregar claims para roles en el futuro???
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8), // Define la duración del token (8 horas como turno?)
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }    
}