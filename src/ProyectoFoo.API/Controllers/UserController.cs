using Microsoft.AspNetCore.Mvc;
using ProyectoFoo.Application.Contracts.Persistence; // Para IUsuarioRepository
using ProyectoFoo.Shared.Models;
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
                // Verificar si el correo electrónico ya existe
                var existingUser = await _usuarioRepository.GetUsuarioPorEmail(model.Email);
                if (existingUser != null)
                {
                    return Conflict("El correo electrónico ya está registrado.");
                }

                // Crear un nuevo usuario
                var newUser = new Usuario(0, model.Name, model.Email, model.Password); // El ID se generará en la base de datos
                                                                                       // La contraseña se hashea dentro del constructor de Usuario

                // Guardar el nuevo usuario en la base de datos
                await _usuarioRepository.AddUsuario(newUser); // Necesitarás añadir este método a tu IUsuarioRepository e implementación

                return Ok("Usuario registrado exitosamente.");
            }
        }
    }
