using Microsoft.IdentityModel.Tokens;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Domain.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProyectoFoo.API.Services
{
    /// <summary>
    /// Servicio responsable de generar tokens de autenticación JWT.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly string _secretKey;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del TokenService.
        /// </summary>
        /// <param name="secretKey">Clave secreta utilizada para firmar los tokens JWT.</param>
        /// <param name="configuration">Configuración de la aplicación (para leer la clave secreta).</param>
        public TokenService(string secretKey, IConfiguration configuration) // Modifica el constructor
        {

            _secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            // Código de depuración temporal
            Console.WriteLine($"TokenService Secret Key (Length: {_secretKey.Length}): {_secretKey}");
        }


        /// <summary>
        /// Genera un token JWT para el usuario proporcionado.
        /// </summary>
        /// <param name="user">El usuario para el que se generará el token.</param>
        /// <returns>Una cadena que representa el token JWT.</returns>
        public string GenerateToken(Usuario user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                // Aquí se pueden agregar claims para roles en el futuro???
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8), // Ajusta la duración si es necesario
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
