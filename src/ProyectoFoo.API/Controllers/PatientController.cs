using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ProyectoFoo.Application.Features.Patients.Search;
using ProyectoFoo.Application.Features.Patients;
using ProyectoFoo.API.Helpers;
using ProyectoFoo.Application.Features.Patients.CRUD.Create;
using ProyectoFoo.Application.Features.Patients.CRUD.Update;
using ProyectoFoo.Application.Features.Patients.Archive;
using ProyectoFoo.Application.Features.Patients.CRUD.Delete;
using ProyectoFoo.Application.Features.Patients.Unarchive;
using ProyectoFoo.Application.Features.Patients.CRUD.Read;
using ProyectoFoo.Domain.Common.Enums;

namespace ProyectoFOO.API.Controllers
{
    /// <summary>
    /// Controlador API para manejar operaciones sobre pacientes.
    /// </summary>
    [Authorize] // Requiere autenticación para todas las acciones en este controlador
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController(IMediator mediator) : ControllerBase
    {

        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));


        /// <summary>
        /// Obtiene la lista de pacientes de la base de datos.
        /// </summary>
        /// <remarks>
        /// Este endpoint devuelve una colección de todos los pacientes.
        ///
        /// Ejemplo de respuesta (200 OK):
        ///     [
        ///         {
        ///             "id": 1,
        ///             "name": "Ana",
        ///             "surname": "Nanana",
        ///             "birthdate": "2005-04-30T00:00:00",
        ///             "identification": "214748357",
        ///             "sex": "Masculino",
        ///             "modality": "Presencial",
        ///             "email": "userrr@example.com",
        ///             "phone": "234566", 
        ///             "age": 20,
        ///             "admissionDate: "2025-05-01T00:00:00",
        ///             "ageRange": "Adulto",
        ///             "nationality": "Colombiano",
        ///             // ... otros campos opcionales del PatientDTO
        ///         },
        ///     ]
        /// </remarks>
        ///  /// <returns>Un objeto <see cref="GetAllPatientsResponse"/> que contiene la lista de pacientes y el estado de la operación.</returns>
        /// <response code="200">Devuelve la lista de pacientes y el estado de la operación.</response>
        /// <response code="401">Usuario no autenticado o ID de usuario no disponible.</response>
        /// <response code="500">Se produjo un error inesperado en el servidor al intentar obtener los pacientes.</response>     
        [HttpGet("pacientes")]
        [ProducesResponseType(typeof(GetAllPatientsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetAllPatientsResponse>> GetPacientes()
        {
            try
            {
                var currentUserId = this.GetCurrentUserId();

                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new ProblemDetails
                    {
                        Title = "No autorizado",
                        Detail = "El ID del usuario no pudo ser extraído del token de autenticación. Asegúrese de que su token es válido y contiene un ID de usuario numérico.",
                        Status = StatusCodes.Status401Unauthorized
                    });
                }

                var query = new GetAllPatientsQuery(currentUserId.Value);
                var response = await _mediator.Send(query);

                if (response.Success)
                {
                    return Ok(response.Patients);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = "Error al obtener pacientes",
                        Detail = response.Message ?? "Se produjo un error al procesar la solicitud.",
                        Status = StatusCodes.Status500InternalServerError
                    });
                }
            }
            catch (Exception)
            {
                return Problem(
                   detail: "Se produjo un error inesperado al procesar su solicitud.",
                   statusCode: StatusCodes.Status500InternalServerError,
                   title: "Error Interno del Servidor"
               );
            }
        }

        /// <summary>
        /// Crea un nuevo paciente  en el sistema.
        /// </summary>
        /// <param name="command">Los datos del paciente a crear. Ver <see cref="CreatePatientCommand"/>.</param>
        /// <remarks>
        /// Ejemplo de solicitud:
        ///
        ///     POST /api/patients
        ///     {
        ///         "nuevoPaciente": {
        ///             "name": "Juan",
        ///             "surname": "Pérez",
        ///             "birthdate": "2000-05-15T00:00:00",
        ///             "nationality" : "Colombiano",
        ///             "typeOfIdentification": "Cédula de Ciudadanía",
        ///             "identification": "123456789",
        ///             "sex": "Masculino",
        ///             "email": "juan.perez@example.com",
        ///             "phone": "3101234567"
        ///             // ... otros campos requeridos u opcionales según CreatePatientCommand
        ///         }
        /// </remarks>
        /// <returns>Una respuesta <see cref="CreatePatientResponse"/> que incluye el ID del paciente creado si la operación fue exitosa.</returns>
        /// <response code="201">Paciente creado exitosamente. Devuelve la ubicación del nuevo recurso y <see cref="CreatePatientResponse"/>.</response>
        /// <response code="400">La solicitud es incorrecta (ej. datos de validación fallidos). Ver <see cref="ValidationProblemDetails"/> o <see cref="ProblemDetails"/>.</response>
        /// <response code="401">Usuario no autenticado o ID de usuario no disponible.</response>
        /// <response code="409">Ya existe un paciente con el número de identificación proporcionado. Ver <see cref="ProblemDetails"/>.</response>
        /// <response code="500">Error interno del servidor. Ver <see cref="ProblemDetails"/>.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreatePatientResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreatePatientResponse>> CreatePaciente([FromBody] CreatePatientCommand command)
        {

            var currentUserId = this.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "No autorizado",
                    Detail = "El ID del usuario no pudo ser extraído del token de autenticación. Asegúrese de que su token es válido y contiene un ID de usuario numérico.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            command.UserId = currentUserId.Value;

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var response = await _mediator.Send(command);
                if (response.Success)
                {
                    return CreatedAtAction(nameof(GetPatientById), new { id = response.PatientId }, response);
                }
                else
                {
                    return BadRequest(new ProblemDetails { Title = "Error al crear el paciente", Detail = response.Message });
                }
            }
            catch (DbUpdateException)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Conflicto de Recurso",
                    Detail = $"Ya existe un paciente con la identificación '{command.Identification}'."
                });
            }
            catch (Exception)
            {
                return Problem(
                     detail: "Se produjo un error inesperado al crear el paciente.",
                     statusCode: StatusCodes.Status500InternalServerError,
                     title: "Error Interno del Servidor"
                 );
            }
        }


        /// <summary>
        /// Obtiene un paciente especifico por su ID.
        /// </summary>
        /// <param name="id">ID del paciente a buscar.</param>
        /// <remarks>
        /// Devuelve los detalles completos del paciente si se encuentra.
        /// El objeto devuelto sería `PatientDTO` dentro de `GetPatientByIdResponse`.
        /// </remarks>
        /// <returns>Un objeto <see cref="PatientDTO"/> si se encuentra; de lo contrario, <see cref="NotFoundResult"/>.</returns>
        /// <response code="200">Devuelve los datos del paciente encontrado (ej. `PatientDTO`).</response>
        /// <response code="404">Paciente no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response> 
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PatientDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetPatientByIdResponse>> GetPatientById(int id)
        {
            try
            {
                var userId = this.GetCurrentUserId();

                if (!userId.HasValue)
                {
                    return Unauthorized("ID de usuario no encontrado o inválido en los claims.");
                }


                var query = new GetPatientByIdQuery(id, userId.Value);
                var response = await _mediator.Send(query);

                if (response.Success && response.Patient != null)
                {
                    return Ok(response.Patient);
                }
                else
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Recurso no encontrado",
                        Detail = response.Message ?? $"Paciente con ID {id} no encontrado.",
                        Status = StatusCodes.Status404NotFound,
                        Instance = HttpContext.Request.Path
                    });
                }
            }
            catch (Exception)
            {
                return Problem(
                     detail: $"Error inesperado al obtener el paciente con ID {id}.",
                     statusCode: StatusCodes.Status500InternalServerError,
                     title: "Error Interno del Servidor"
                 );
            }
        }


        /// <summary>
        /// Actualiza parcialmente los datos de un paciente existente.
        /// </summary>
        /// <param name="id">ID del paciente a actualizar.</param>
        /// <param name="command">Objeto JSON con los campos a actualizar.</param>
        /// <remarks>
        /// Permite actualizar los campos proporcionados en el cuerpo de la solicitud.
        /// El ID del paciente se especifica en la URL y debe coincidir con el command.Id si este último existe.
        /// Solo dejar los campos que van a ser cambiados.
        /// </remarks>
        /// /// <returns>Una respuesta <see cref="UpdatePatientResponse"/> o <see cref="NoContentResult"/> en caso de éxito.</returns>
        /// <response code="200">Paciente actualizado exitosamente. Devuelve <see cref="UpdatePatientResponse"/>.</response>
        /// <response code="204">Paciente actualizado exitosamente (si no se devuelve contenido).</response>
        /// <response code="400">Solicitud incorrecta (ej. datos inválidos).</response>
        /// <response code="404">Paciente no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType(typeof(UpdatePatientResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientCommand command)
        {
            var userId = this.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("ID de usuario no encontrado o inválido en los claims.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            command.Id = id;
            command.UserId = userId.Value;

            try
            {
                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    return Ok(new { message = $"Paciente con Id #{id} fue actualizado con éxito." });
                }
                else
                {
                    return NotFound(new ProblemDetails { Title = "No encontrado", Detail = response.Message, Status = StatusCodes.Status404NotFound });
                }
            }
            catch (Exception)
            {
                return Problem(
                     detail: $"Error inesperado al actualizar el paciente con ID {id}.",
                     statusCode: StatusCodes.Status500InternalServerError,
                     title: "Error Interno del Servidor"
                 );
            }
        }


        /// <summary>
        /// Elimina un paciente existente por su ID.
        /// </summary>
        /// <remarks>
        /// Esta operación elimina permanentemente al paciente.
        /// Ejemplo de solicitud: `DELETE /api/Patient/5`
        /// </remarks>
        /// <returns>No devuelve contenido en caso de éxito.</returns>
        /// <response code="204">Paciente eliminado exitosamente.</response>
        /// <response code="404">Paciente no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePatient(int id)
        {

            try
            {
                var userId = this.GetCurrentUserId();

                if (!userId.HasValue)
                {
                    return Unauthorized("ID de usuario no encontrado o inválido en los claims.");
                }

                var command = new DeletePatientCommand { PatientId = id, UserId = userId.Value };

                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Recurso no encontrado",
                        Detail = response.Message ?? $"Paciente con ID {id} no encontrado para eliminar.",
                        Status = StatusCodes.Status404NotFound,
                        Instance = HttpContext.Request.Path
                    });
                }
            }
            catch (Exception)
            {
                return Problem(
                    detail: $"Error inesperado al eliminar el paciente con ID {id}.",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Error Interno del Servidor"
                );
            }
        }


        // --- RELACIONADO A ARCHIVAR/DESARCHIVAR Y OBTENER ARCHIVADOS ---


        /// <summary>
        /// Archiva (desactiva) a un paciente activo por su ID.
        /// </summary>
        /// <param name="id">ID del paciente a archivar.</param>
        /// <returns>Una respuesta <see cref="ArchivePatientResponse"/> indicando el resultado.</returns>
        /// <response code="200">El paciente fue archivado correctamente. Devuelve <see cref="ArchivePatientResponse"/>.</response>
        /// <response code="404">No se encontró el paciente o ya estaba archivado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [ProducesResponseType(typeof(ArchivePatientResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPut("pacientes/{id}/archive")]
        public async Task<ActionResult<ArchivePatientResponse>> ArchivePaciente(int id)
        {
            try
            {
                var userId = this.GetCurrentUserId();

                if (!userId.HasValue)
                {
                    return Unauthorized("ID de usuario no encontrado o inválido en los claims.");
                }

                var command = new ArchivePatientCommand(id, userId.Value);
                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    return Ok(new { message = response.Message });
                }
                else
                {
                    return NotFound(new ProblemDetails { Title = "Operación fallida o recurso no encontrado", Detail = response.Message, Status = StatusCodes.Status404NotFound });
                }
            }
            catch (Exception)
            {
                return Problem(detail: $"Error al archivar paciente con ID {id}.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }
        }

        /// <summary>
        /// Desarchiva (reactiva lógicamente) a un paciente previamente archivado.
        /// </summary>
        /// <param name="id">ID del paciente a desarchivar.</param>
        /// <returns>Mensaje indicando si la operación fue exitosa.</returns>
        /// <response code="200">El paciente fue desarchivado correctamente. Devuelve la respuesta del comando.</response>
        /// <response code="404">No se encontró el paciente o no estaba archivado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("pacientes/{id}/unarchive")]
        [ProducesResponseType(typeof(ArchivePatientResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ArchivePatientResponse>> UnarchivePaciente(int id)
        {
            try
            {
                var userId = this.GetCurrentUserId();

                if (!userId.HasValue)
                {
                    return Unauthorized("ID de usuario no encontrado o inválido en los claims.");
                }
                var command = new UnarchivePatientCommand(id, userId.Value);
                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    return Ok(new { message = response.Message });
                }
                else
                {
                    return NotFound(new ProblemDetails { Title = "Operación fallida o recurso no encontrado", Detail = response.Message, Status = StatusCodes.Status404NotFound });
                }
            }
            catch (Exception)
            {
                return Problem(detail: $"Error al desarchivar paciente con ID {id}.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }

        }


        /// <summary>
        /// Obtiene todos los pacientes que están deshabilitados (IsEnabled = false) con toda su información.
        /// </summary>
        /// <remarks>Devuelve una lista de `PatientDTO` para los pacientes archivados.</remarks>
        /// <returns>Una lista de <see cref="PatientDTO"/> de pacientes archivados.</returns>
        /// <response code="200">Retorna la lista de pacientes archivados.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("patients/archived")]
        [ProducesResponseType(typeof(List<PatientDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PatientDTO>>> GetAllArchivedPatients()
        {
            try
            {
                var userId = this.GetCurrentUserId();

                if (!userId.HasValue)
                {
                    return Unauthorized("ID de usuario no encontrado o inválido en los claims.");
                }

                var query = new GetAllArchivedPatientsCommand(userId.Value);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception)
            {
                return Problem(detail: "Error al obtener pacientes archivados.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }
        }


        //ENDPOINT DE BUSQUEDA 

        /// <summary>
        /// Búsqueda y filtro de pacientes según necesidad.
        /// </summary>
        /// <param name="identification">Número de identificación del paciente a buscar (como parámetro de consulta).</param>
        /// <param name="fullName">Nombre del paciente a buscar (como parámetro de consulta).</param>
        /// <param name="email">Email del paciente a buscar (como parámetro de consulta).</param>
        /// <param name="nationality">Nacionalidad a filtrar (como parámetro de consulta).</param>
        /// <param name="sexType">Tipo de sexo a filtrar (como parámetro de consulta).</param>
        /// <param name="modality">Tipo de modalidad del paciente a filtrar (como parámetro de consulta).</param>
        /// <param name="ageRange">Rango etario del paciente a buscar (como parámetro de consulta).</param>>
        /// <returns>Un <see cref="PatientDTO"/> que indica el resultado.</returns>
        /// <response code="200">Paciente encontrado exitosamente.</response>
        /// <response code="400">Error en la solicitud.</response>
        /// <response code="404">No se encontró ningún paciente con el número de identificación proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Search(
         [FromQuery] string? fullName,
        [FromQuery] string? identification,
        [FromQuery] string? email,
        [FromQuery] string? nationality,
        [FromQuery] SexType? sexType,
        [FromQuery] ModalityType? modality,
        [FromQuery] string? ageRange)
        {
            var query = new SearchPatientsQuery(
            fullName,
            identification,
            email,
            nationality,
            sexType,
            modality,
            ageRange
        );
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
