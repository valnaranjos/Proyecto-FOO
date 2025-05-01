using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using ProyectoFoo.Infrastructure.Context;
using ServiceStack.Text.Json;
using MySqlConnector;
using Microsoft.AspNetCore.Authorization;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.API.Models.Dtos;
using MediatR;
using ProyectoFoo.Application.Features.Patients;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace ProyectoFOO.API.Controllers
{
    /// <summary>
    /// Controlador API para manejar operaciones sobre pacientes.
    /// </summary>
    //[Authorize] // Requiere autenticaci�n para todas las acciones en este controlador
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {

        private readonly IMediator _mediator;

        public PatientController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }



        /// <summary>
        /// Obtiene la lista de pacientes de la base de datos.
        /// </summary>
        /// /// <returns>Respuesta HTTP con una lista de todos los pacientes.</returns>
        /// <remarks>
        /// Este endpoint devuelve una colecci�n de todos los pacientes actualmente registrados en el sistema.
        ///
        /// Ejemplo de solicitud:
        ///
        ///     GET /api/patients
        ///
        /// Ejemplo de respuesta (200 OK):
        ///
        ///     [
        ///         {
        ///             "id": 1,
        ///             "name": "Ana",
        ///             "surname": "Nanana",
        ///             "birthdate": "2005-04-30T00:00:00",
        ///             "identification": 214748357,
        ///             "sex": "M",
        ///             "modality": "Presencial",
        ///             "email": "userrr@example.com",
        ///             "phone": "234566"
        ///         },
        ///         {
        ///             "id": 2,
        ///             "name": "string",
        ///             "surname": "string",
        ///             "birthdate": "2025-05-01T00:00:00",
        ///             "identification": 1234569574,
        ///             "sex": "F",
        ///             "modality": "string",
        ///             "email": "user@example.com",
        ///             "phone": "3008644707"
        ///         },
        ///         {
        ///             "id": 3,
        ///             "name": "Juan",
        ///             "surname": "Perez",
        ///             "birthdate": "2025-05-01T00:00:00",
        ///             "identification": 105386074,
        ///             "sex": "M",
        ///             "modality": "Presencial",
        ///             "email": "juan@example.com",
        ///             "phone": "3105468820"
        ///         }
        ///     ]
        ///
        /// Ejemplo de respuesta (500 Internal Server Error - Error al obtener pacientes):
        ///
        ///     {
        ///         "message": "Error al obtener todos los pacientes."
        ///     }
        /// </remarks>
        [HttpGet("pacientes")]
        public async Task<ActionResult<GetAllPatientsResponse>> GetPacientes()
        {
            try
            {
                var query = new GetAllPatientsQuery();
                var response = await _mediator.Send(query);

                if (response.Success)
                {
                    return Ok(response.Patients);
                }
                else
                {
                    return StatusCode(500, response.Message ?? "Error al obtener todos los pacientes.");
                }
            }
            catch (Exception ex)
            {
                // En caso de error, devuelve un BadRequest
                return BadRequest($"Error al obtener los pacientes: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo paciente.
        /// </summary>
        /// <param name="request">Objeto JSON que representa la informaci�n del nuevo paciente.</param>
        /// <returns>Respuesta HTTP indicando el �xito de la creaci�n. En caso de �xito, devuelve la informaci�n del paciente creado.</returns>
        /// <remarks>
        /// Este endpoint recibe la informaci�n para crear un nuevo paciente en el sistema.
        ///
        /// Ejemplo de solicitud:
        ///
        ///     POST /api/patients
        ///     {
        ///         "nuevoPaciente": {
        ///             "name": "Juan",
        ///             "surname": "P�rez",
        ///             "birthdate": "2000-05-15T00:00:00",
        ///             "identification": 123456789,
        ///             "sex": "M",
        ///             "modality": "Virtual",
        ///             "email": "juan.perez@example.com",
        ///             "phone": "3101234567"
        ///             +datos opcionales...   
        ///         }
        ///     }
        ///
        /// Ejemplo de respuesta (201 Created):
        ///
        ///     {
        ///         "patient": {
        ///             "id": 4,
        ///             "name": "Juan",
        ///             "surname": "P�rez",
        ///             "birthdate": "2000-05-15T00:00:00",
        ///             "identification": 123456789,
        ///             "sex": "M",
        ///             "modality": "Virtual",
        ///             "email": "juan.perez@example.com",
        ///             "phone": "3101234567"
        ///         },
        ///         "success": true
        ///     }
        ///
        /// Ejemplo de respuesta (400 Bad Request - Validaci�n fallida):
        ///
        ///     {
        ///         "type": "URL...",
        ///         "title": "One or more validation errors occurred.",
        ///         "status": 400,
        ///         "errors": {
        ///             "NuevoPaciente.Name": [
        ///                 "The Name field is required."
        ///             ]
        ///         }
        ///     }
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<CreatePatientResponse>> CreatePaciente([FromBody] CreatePatientCommand command)
        {
            var response = await _mediator.Send(command);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (response.Success)
                {
                    return CreatedAtAction(nameof(GetPatientById), new { id = response.PatientId }, response);
                }
                else
                {
                    return BadRequest(response); // O podr�as usar StatusCode seg�n el tipo de error
                }
            }
            catch (DbUpdateException ex) // Captura excepciones relacionadas con la base de datos
            {
                if (ex.InnerException is MySqlException mySqlException && mySqlException.Number == 1062) // 1062 es el c�digo de error para violaci�n de unicidad en MySQL
                {
                    return Conflict($"Ya existe un paciente con la identificaci�n '{command.Identification}'."); // Usamos command.Identification ahora
                }

                return BadRequest($"Error al crear el paciente: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado al crear el paciente: {ex.Message}"); // Usamos StatusCode 500 para errores inesperados
            }
        }


        /// <summary>
        /// Obtiene un paciente especifico por su ID.
        /// </summary>
        /// <param name="id">ID del paciente a buscar.</param>
        /// <returns>Respuesta HTTP con la informaci�n del paciente si se encuentra.</returns>
        /// <remarks>
        /// Este endpoint permite obtener los detalles de un paciente espec�fico utilizando su identificador �nico.
        ///
        /// Ejemplo de solicitud:
        ///
        ///     GET /api/patients/1
        ///
        /// Ejemplo de respuesta (200 OK):
        ///
        ///     {
        ///         "id": 1,
        ///         "name": "Ana",
        ///         "surname": "Nanana",
        ///         "birthdate": "2005-04-30T00:00:00",
        ///         "identification": 214748357,
        ///         "sex": "M",
        ///         "modality": "Presencial",
        ///         "email": "userrr@example.com",
        ///         "phone": "234566"
        ///     }
        ///
        /// Ejemplo de respuesta (404 Not Found - Paciente no encontrado):
        ///
        ///     {
        ///         "success": false,
        ///         "message": "Paciente con ID 99 no encontrado."
        ///     }
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetPatientByIdResponse>> GetPatientById(int id)
        {
            try
            {
                var query = new GetPatientByIdQuery(id);
                var response = await _mediator.Send(query);

                if (response.Success && response.Patient != null)
                {
                    return Ok(response.Patient);
                }
                else
                {
                    return NotFound(response.Message ?? $"Paciente con ID {id} no encontrado.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado al obtener el paciente con ID {id}: {ex.Message}");
            }
        }


        /// <summary>
        /// Actualiza parcialmente los datos de un paciente existente.
        /// </summary>
        /// <param name="id">ID del paciente a actualizar.</param>
        /// <param name="command">Objeto JSON con los campos a actualizar.</param>
        /// <returns>Respuesta HTTP indicando el �xito o error de la actualizaci�n. En caso de �xito, devuelve el paciente actualizado.</returns>
        /// <remarks>
        /// Este endpoint permite actualizar solo los campos proporcionados en el cuerpo de la solicitud.
        /// El ID del paciente se debe especificar en la URL.
        ///
        /// Ejemplo de solicitud:
        ///
        ///     PUT /api/1
        ///     {
        ///         "name": "Nuevo Nombre",
        ///         "phone": "987654321"
        ///     }
        ///
        /// Ejemplo de respuesta (204 No Content - �xito):
        ///
        ///     // Sin cuerpo de respuesta
        ///     
        /// Ejemplo de respuesta (404 Not Found - Paciente no encontrado):
        ///
        ///     {
        ///         "message": "Paciente con ID 1 no encontrado."
        ///     }
        /// </remarks>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePaciente(int id, [FromBody] UpdatePatientCommand command)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            command.Id = id;

            try
            {
                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    return NoContent(); // C�digo 204 para indicar una actualizaci�n exitosa sin devolver el recurso
                }
                else
                {
                    return NotFound(response.Message);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Los datos del paciente han sido modificados por otro usuario.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar el paciente: {ex.Message}");
            }
        }


        /// <summary>
        /// Elimina un paciente existente por su ID.
        /// </summary>
        /// <param name="id">ID del paciente a eliminar.</param>
        /// <returns>Respuesta HTTP indicando el �xito o error de la eliminaci�n.</returns>
        /// <remarks>
        /// Este endpoint permite eliminar un paciente del sistema utilizando su identificador �nico.
        ///
        /// Ejemplo de solicitud:
        ///
        ///     DELETE /api/patients/5
        ///
        /// Ejemplo de respuesta (204 No Content - �xito):
        ///
        ///     // Sin cuerpo de respuesta
        ///
        /// Ejemplo de respuesta (404 Not Found - Paciente no encontrado):
        ///
        ///     {
        ///         "message": "Paciente con ID 5 no encontrado."
        ///     }
        ///
        /// Ejemplo de respuesta (500 Internal Server Error - Error al eliminar):
        ///
        ///     {
        ///         "message": "Error inesperado al eliminar el paciente con ID 5: Error de base de datos."
        ///     }
        /// </remarks>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePaciente(int id)
        {
            var command = new DeletePatientCommand { PatientId = id };

            try
            {
                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    return NoContent(); // C�digo 204 para indicar una eliminaci�n exitosa
                }
                else
                {
                    return NotFound(response.Message); // C�digo 404 si el paciente no se encuentra
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error inesperado al eliminar el paciente: {ex.Message}");
            }
        }

    }
}