using Microsoft.AspNetCore.Mvc;
using ProyectoFoo.Application.Contracts.Persistence; // Para IUsuarioRepository
using ProyectoFoo.API.Models.Authentication;
using ProyectoFoo.Domain.Entities;
using MediatR;
using ProyectoFoo.Application.Features.Users;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Necesario para StatusCodes

namespace ProyectoFoo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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

        // [HttpGet("{id:int}")]
        // public async Task<ActionResult<GetUserByIdResponse>> GetUserById(int id)
        // {
        //     // ... implementación ...
        // }
    }
}
