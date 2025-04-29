using Microsoft.AspNetCore.Mvc;
using ProyectoFoo.Application.Contracts.Persistence; // Para IUsuarioRepository
using ProyectoFoo.API.Models.Authentication;
using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _usuarioRepository;

        public UserController(IUserRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Devuelve errores de validación
            }

            // Verificar si el correo electrónico ya existe
            var existingUser = await _usuarioRepository.GetUsuarioPorEmail(model.Email);
            if (existingUser != null)
            {
                return Conflict("El correo electrónico ya está registrado.");
            }

            // Crear un nuevo usuario
            var newUser = new Usuario()
            {
                Identificacion = model.Identification,
                Name = model.Name,
                Apellido = model.Surname,
                Email = model.Email,
                LastAccesDate = DateTime.UtcNow
            };

            // Llama al método HashPassword
            string hashedPassword = newUser.HashPassword(model.Password);

            // Utiliza un método dentro de la clase Usuario para establecer el PasswordHash
            newUser.SetPasswordHash(hashedPassword);

            // Guardar el nuevo usuario en la base de datos Usuarios.
            await _usuarioRepository.AddUsuario(newUser); 

            return Ok("Usuario registrado exitosamente.");
        }
    }
}
