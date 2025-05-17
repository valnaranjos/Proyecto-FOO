using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.Features.Patients.Search;
using ProyectoFoo.Application.Features.Patients.Filters;
using ProyectoFoo.Application.Features.Patients.CRUD;
using ProyectoFoo.Shared.Models;
using ProyectoFoo.Application.Features.PatientMaterials;
using ProyectoFoo.Application.Features.Patients;
using ProyectoFoo.Application.Features.Notes;
using ProyectoFoo.Application.Features.Notes.Queries;
using ProyectoFoo.Application.Features.Notes.Handlers;
using System.Security.Claims;

namespace ProyectoFOO.API.Controllers
{
    /// <summary>
    /// Controlador API para manejar operaciones sobre pacientes.
    /// </summary>
    [Authorize] // Requiere autenticación para todas las acciones en este controlador
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController(IMediator mediator, IPatientService patientService) : ControllerBase
    {

        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        private readonly IPatientService _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));



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
        /// <response code="500">Se produjo un error inesperado en el servidor al intentar obtener los pacientes.</response>     
        [HttpGet("pacientes")]
        [ProducesResponseType(typeof(GetAllPatientsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetAllPatientsResponse>> GetPacientes()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var query = new GetAllPatientsQuery(userId);
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
        /// /// <returns>Una respuesta <see cref="CreatePatientResponse"/> que incluye el ID del paciente creado si la operación fue exitosa.</returns>
        /// <response code="201">Paciente creado exitosamente. Devuelve la ubicación del nuevo recurso y <see cref="CreatePatientResponse"/>.</response>
        /// <response code="400">La solicitud es incorrecta (ej. datos de validación fallidos). Ver <see cref="ValidationProblemDetails"/> o <see cref="ProblemDetails"/>.</response>
        /// <response code="409">Ya existe un paciente con el número de identificación proporcionado. Ver <see cref="ProblemDetails"/>.</response>
        /// <response code="500">Error interno del servidor. Ver <see cref="ProblemDetails"/>.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreatePatientResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreatePatientResponse>> CreatePaciente([FromBody] CreatePatientCommand command)
        {
            var response = await _mediator.Send(command);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                if (response.Success)
                {
                    return CreatedAtAction(nameof(GetPatientById), new { id = response.PatientId }, response);
                }
                else
                {
                    return BadRequest(new ProblemDetails { Title = "Error al crear el paciente", Detail = response.Message });
                }
            }
            catch (DbUpdateException) // Captura excepciones relacionadas con la base de datos
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
        [ProducesResponseType(typeof(PatientDTO), StatusCodes.Status200OK)] // Asumiendo que response.Patient es de tipo PatientDTO
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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
        /// [ProducesResponseType(typeof(UpdatePatientResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientCommand command)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            command.Id = id;

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
        /// Esta operación elimina permanentemente (o lógicamente, según implementación) al paciente.
        ///
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
            var command = new DeletePatientCommand { PatientId = id };

            try
            {
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
        ///  [ProducesResponseType(typeof(ArchivePatientResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPut("pacientes/{id}/archive")]
        public async Task<ActionResult<ArchivePatientResponse>> ArchivePaciente(int id)
        {
            try
            {
                var command = new ArchivePatientCommand(id);
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
        [ProducesResponseType(typeof(ArchivePatientResponse), StatusCodes.Status200OK)] // Asumiendo que Unarchive usa el mismo tipo de respuesta
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ArchivePatientResponse>> UnarchivePaciente(int id)
        {
            try
            {
                var command = new UnarchivePatientCommand(id);
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
                var query = new GetAllArchivedPatientsCommand();
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception)
            {
                return Problem(detail: "Error al obtener pacientes archivados.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }
        }



        //RELACIONADO A MATERIAL DE PACIENTE:


        /// <summary>
        /// Crea un nuevo material para un paciente específico.
        /// </summary>
        /// <param name="patientId">Identificador único del paciente.</param>
        /// <param name="createPacienteMaterialDto">Datos para la creación del material.</param>
        /// <returns>Un ActionResult que indica el resultado de la creación.</returns>
        /// <response code="201">Retorna el material recién creado.</response>
        /// <response code="400">Si la petición no es válida.</response>
        /// <response code="404">Si el paciente especificado no existe.</response>
        ///  /// <response code="500">Error interno del servidor.</response>
        [HttpPost("{patientId}/materials")]
        [ProducesResponseType(typeof(CreatePatientMaterialResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreatePatientMaterialResponse>> CreatePatientMaterial(
            [FromRoute] int patientId,
            [FromBody] CreatePatientMaterialDto createPacienteMaterialDto)
        {
            if (createPacienteMaterialDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var command = new CreatePatientMaterialCommand
                {
                    PatientId = patientId,
                    Material = createPacienteMaterialDto
                };

                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    return CreatedAtAction(nameof(GetPatientMaterialById),
                        new { patientId, materialId = response.PatientMaterial.Id },
                        response);
                }

                if (response.Message.Contains("No se encontró el paciente"))
                {
                    return NotFound(new ProblemDetails { Title = "Paciente no encontrado", Detail = response.Message, Status = StatusCodes.Status404NotFound });
                }

                return BadRequest(new ProblemDetails { Title = "Error al crear material", Detail = response.Message ?? "No se pudo crear el material." });
            }
            catch (Exception)
            {
                // Loggear ex
                return Problem(detail: "Error inesperado al crear material para el paciente.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }
        }



        /// <summary>
        /// Obtiene todo el material asociado a un paciente específico.
        /// </summary>
        /// <param name="patientId">Identificador único del paciente.</param>
        /// <returns>Una lista de <see cref="PatientMaterialDto"/> para el paciente.</returns>
        /// <response code="200">Retorna la lista de materiales del paciente. Puede ser una lista vacía si no tiene materiales.</response>
        /// <response code="404">Si el paciente especificado no existe (esto debería ser manejado por el comando idealmente).</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{patientId}/materials")]
        [ProducesResponseType(typeof(List<PatientMaterialDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PatientMaterialDto>>> GetAllPatientMaterials([FromRoute] int patientId)
        {
            try
            {
                var query = new GetAllPatientMaterialsCommand { PatientId = patientId };
                var response = await _mediator.Send(query);

                if (response != null && response.Count > 0)
                {
                    return Ok(response);
                }

                var patientExists = await _mediator.Send(new GetPatientByIdQuery(patientId));
                if (patientExists == null)
                {
                    return NotFound($"No se encontró el paciente con ID: {patientId}");
                }

                return Ok(new List<PatientMaterialDto>());
            }
            catch (Exception)
            {
                return Problem(detail: "Error inesperado al obtener los materiales del paciente.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }
        }


        /// <summary>
        /// Obtiene un material específico por su ID para un paciente específico.
        /// </summary>
        /// <param name="patientId">Identificador único del paciente.</param>
        /// <param name="materialId">Identificador único del material.</param>
        /// <returns>El <see cref="PatientMaterialDto"/> solicitado.</returns>
        /// <response code="200">Retorna el material solicitado.</response>
        /// <response code="404">Si el paciente o el material no existen.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{patientId}/materials/{materialId}")]
        [ProducesResponseType(typeof(PatientMaterialDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PatientMaterialDto>> GetPatientMaterialById([FromRoute] int patientId, [FromRoute] int materialId)
        {
            try
            {
                var query = new GetPatientMaterialByIdQuery { PatientId = patientId, MaterialId = materialId };
                var materialDto = await _mediator.Send(query);

                if (materialDto != null)
                {
                    return Ok(materialDto);
                }

                return NotFound(new ProblemDetails
                {
                    Title = "Recurso no encontrado",
                    Detail = $"No se encontró el material con ID {materialId} para el paciente con ID {patientId}.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception)
            {
                return Problem(detail: "Error inesperado al obtener el material del paciente.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }
        }


        /// <summary>
        /// Actualiza la información de un material específico de un paciente.
        /// </summary>
        /// <param name="patientId">Identificador único del paciente.</param>
        /// <param name="materialId">Identificador único del material a actualizar.</param>
        /// <param name="updatePatientMaterialDto">Datos para la actualización del material.</param>
        /// <returns>Un <see cref="UpdatePatientMaterialResponse"/> que indica el resultado.</returns>
        /// <response code="200">Retorna <see cref="UpdatePatientMaterialResponse"/> con el material actualizado o mensaje de éxito.</response>
        /// <response code="400">Si la petición no es válida (ej. datos faltantes).</response>
        /// <response code="404">Si el paciente o el material no existen.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{patientId}/materials/{materialId}")]
        [ProducesResponseType(typeof(UpdatePatientMaterialResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UpdatePatientMaterialResponse>> UpdatePatientMaterial(
        [FromRoute] int patientId,
        [FromRoute] int materialId,
        [FromBody] UpdatePatientMaterialDto updatePatientMaterialDto)
        {
            if (updatePatientMaterialDto == null || !ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var command = new UpdatePatientMaterialCommand
                {
                    PatientId = patientId,
                    MaterialId = materialId,
                    Material = updatePatientMaterialDto
                };

                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    if (response.Message != null && (response.Message.Contains("no encontrado", StringComparison.CurrentCultureIgnoreCase) || response.Message.Contains("no existe", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        return NotFound(new ProblemDetails { Title = "Recurso no encontrado", Detail = response.Message, Status = StatusCodes.Status404NotFound });
                    }
                    return BadRequest(new ProblemDetails { Title = "Error al actualizar material", Detail = response.Message ?? "No se pudo actualizar el material." });
                }
            }
            catch (Exception)
            {
                return Problem(detail: "Error inesperado al actualizar el material del paciente.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }
        }


        /// <summary>
        /// Elimina un material específico de un paciente.
        /// </summary>
        /// <param name="patientId">Identificador único del paciente.</param>
        /// <param name="materialId">Identificador único del material a eliminar.</param>
        /// <returns>No devuelve contenido en caso de éxito.</returns>
        /// <response code="204">Si el material fue eliminado exitosamente.</response>
        /// <response code="404">Si el paciente o el material no existen.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{patientId}/materials/{materialId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePatientMaterial(
        [FromRoute] int patientId,
        [FromRoute] int materialId)
        {
            try
            {
                var command = new DeletePatientMaterialCommand
                {
                    PatientId = patientId,
                    MaterialId = materialId
                };

                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    return NoContent();
                }

                return NotFound(new ProblemDetails { Title = "Recurso no encontrado", Detail = response.Message ?? "No se pudo eliminar el material.", Status = StatusCodes.Status404NotFound });
            }
            catch (Exception)
            {
                return Problem(detail: "Error inesperado al eliminar el material del paciente.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }
        }


        //ENDPOINTS DE BUSQUEDA 

        /// <summary>
        /// Busca un paciente por su correo electrónico.
        /// </summary>
        /// <param name="email">Correo electrónico del paciente.</param>
        /// <returns>Información del paciente si se encuentra.</returns>
        /// <response code="200">Paciente encontrado exitosamente.</response>
        /// <response code="404">No se encontró un paciente con el correo electrónico proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("search-by-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPatientByEmail(string email)
        {
            var paciente = await _patientService.GetPatientByEmailAsync(email);
            if (paciente == null)
            {
                return NotFound($"No se encontró un paciente con el email: {email}.");
            }
            return Ok(paciente);
        }


        /// <summary>
        /// Busca un paciente por su número de identificación.
        /// </summary>
        /// <param name="identification">Número de identificación del paciente a buscar (como parámetro de consulta).</param>
        /// <returns>Respuesta HTTP con la información del paciente encontrado o NotFound si no existe.</returns>
        /// <response code="200">Paciente encontrado exitosamente.</response>
        /// <response code="400">Error en la solicitud.</response>
        /// <response code="404">No se encontró ningún paciente con el número de identificación proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("search-by-identification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetPatientByIdResponse>> GetPatientByIdentification([FromQuery] string identification)
        {
            var query = new GetPatientByIdentificationCommand(identification);
            var response = await _mediator.Send(query);

            if (response.Success && response.Patient != null)
            {
                return Ok(response.Patient);
            }
            else if (response.Success)
            {
                return NotFound($"No se encontró ningún paciente con la identificación: {identification}.");
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        /// <summary>
        /// Busca un paciente por su nacionalidad.
        /// </summary>
        /// <param name="nationality"> String nacionalidad del paciente(como parámetro de consulta).</param>
        /// <returns>Lista de pacientes con la nacionalidad especificada.</returns>
        /// <response code="200">Pacientes encontrados exitosamente.</response>
        /// <response code="400">Error en la solicitud.</response>
        /// <response code="404">No se encontró ningún paciente con la nacionalidad proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("search-by-nationality")]
        public async Task<ActionResult<GetPatientsByNationalityResponse>> GetPatientsByNationality([FromQuery] string nationality)
        {
            var query = new GetPatientsByNationalityCommand(nationality);
            var response = await _mediator.Send(query);

            if (response.Success && response.Patients != null)
            {
                return Ok(response.Patients);
            }
            else if (response.Success)
            {
                return NotFound($"No se encontraron pacientes con nacionalidad: {nationality}.");
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        /// <summary>
        /// Busca un paciente por su nombre.
        /// </summary>
        /// <param name="fullname"> String nombre del paciente(como parámetro de consulta).</param>
        /// <returns>Lista de pacientes que coinciden con el nombre completo.</returns>
        /// <response code="200">Paciente encontrado exitosamente.</response>
        /// <response code="400">Error en la solicitud.</response>
        /// <response code="404">No se encontró ningún paciente con la nacionalidad proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("search-by-fullname")]
        public async Task<ActionResult<GetPatientsByNationalityResponse>> GetPatientsByFullName([FromQuery] string fullname)
        {
            if (string.IsNullOrWhiteSpace(fullname))
            {
                return BadRequest("El nombre completo no puede estar vacío.");
            }

            var query = new GetPatientsByFullNameCommand(fullname.Trim());
            var response = await _mediator.Send(query);

            if (response.Success && response.Patients != null && response.Patients.Count != 0)
            {
                return Ok(response.Patients);
            }
            else
            {
                return NotFound($"No se encontraron pacientes con el nombre completo: {fullname.Trim()}");
            }
        }

        //FILTROS 

        /// <summary>
        /// Filtra los pacientes por tipo de sexo.
        /// </summary>
        /// <param name="sex">Tipo de sexo del paciente.</param>
        /// <returns>Lista de pacientes filtrados por tipo de sexo.</returns>
        /// <returns>Respuesta HTTP con la lista de pacientes que coinciden con la el tipo de sexo proporcionada o NotFound si no existe ninguno.</returns>
        /// <response code="200">Lista de pacientes filtrados exitosamente.</response>
        /// <response code="400">Error en la solicitud.</response>
        /// <response code="404">No se encontraron pacientes con el tipo de sexo proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("filter-by-sexType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetPatientsBySexTypeResponse>> GetPatientsBySex([FromQuery] string sex)
        {
            var query = new GetPatientsBySexTypeCommand(sex);
            var response = await _mediator.Send(query);

            if (response.Success && response.Patients != null)
            {
                return Ok(response.Patients);
            }
            else if (response.Success)
            {
                return NotFound($"No se encontraron pacientes con tipo de sexo {sex}");
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        /// <summary>
        /// Filtra los pacientes por modalidad.
        /// </summary>
        /// <param name="modality">Modalidad del paciente.</param>
        /// <returns>Lista de pacientes filtrados por modalidad.</returns>
        ///  <returns>Respuesta HTTP con la lista de pacientes que coinciden con la modalidad proporcionada o NotFound si no existe ninguno.</returns>
        /// <response code="200">Lista de pacientes filtrados exitosamente.</response>
        /// <response code="400">Error en la solicitud.</response>
        /// <response code="404">No se encontraron pacientes con la modalidad proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("filter-by-modality")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPatientByModality(string modality)
        {
            var patients = await _patientService.GetPacientesByModalityAsync(modality);
            if (patients == null || patients.Count == 0)
            {
                return NotFound("No se encontraron pacientes con esa modalidad.");
            }
            return Ok(patients);
        }


        /// <summary>
        /// Filtra pacientes por rango etario.
        /// </summary>
        /// <param name="ageRange">Rango etario de los pacientes a filtrar (como parámetro de consulta).</param>
        /// <returns>Respuesta HTTP con la lista de pacientes que coinciden con el rango etario proporcionado o NotFound si no existe ninguno.</returns>
        /// <response code="200">Lista de pacientes filtrados exitosamente.</response>
        /// <response code="400">Error en la solicitud.</response>
        /// <response code="404">No se encontraron pacientes en el rango etario proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("filter-by-age-range")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetPatientsByAgeRangeResponse>> GetPatientsByAgeRange([FromQuery] string ageRange)
        {
            var query = new GetPatientsByAgeRangeCommand(ageRange);
            var response = await _mediator.Send(query);

            if (response.Success && response.Patients != null)
            {
                return Ok(response.Patients);
            }
            else if (response.Success)
            {
                return NotFound($"No se encontraron pacientes {ageRange} de rango etario.");
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
        /// <summary>
        /// Crea una nueva nota para un paciente.
        /// </summary>
        /// <param name="patientId">ID del paciente.</param>
        /// <param name="note">Datos de la nota a crear.</param>
        /// <returns>La nota recién creada.</returns>
        /// <response code="201">Nota creada exitosamente.</response>
        /// <response code="400">Datos inválidos.</response>
        [HttpPost("{patientId}/notes")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NoteResponseDto>> CreateNote(int patientId, [FromBody] CreateNoteDto note)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            note.PatientId = patientId;

            var command = new CreateNoteCommand { Note = note };
            var createdNote = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetNoteById), new { patientId, noteId = createdNote.PatientNote.Id }, createdNote);
        }

        /// <summary>
        /// Obtiene una nota específica de un paciente.
        /// </summary>
        /// <param name="patientId">ID del paciente.</param>
        /// <param name="noteId">ID de la nota.</param>
        /// <returns>La nota solicitada.</returns>
        /// <response code="200">Nota encontrada exitosamente.</response>
        /// <response code="404">Nota no encontrada.</response>
        [HttpGet("{patientId}/notes/{noteId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NoteResponseDto>> GetNoteById(int patientId, int noteId)
        {
            var query = new GetNoteByIdQuery(noteId);
            var note = await _mediator.Send(query);

            if (note == null)
                return NotFound($"No se encontró la nota con ID {noteId} para el paciente {patientId}");

            if (note.PatientId != patientId)
                return NotFound($"La nota con ID {noteId} no pertenece al paciente {patientId}");

            return Ok(note);
        }


        /// <summary>
        /// Obtiene todas las notas de un paciente específico.
        /// </summary>
        /// <param name="patientId">ID del paciente.</param>
        /// <returns>Lista de notas del paciente.</returns>
        /// <response code="200">Notas encontradas exitosamente.</response>
        [HttpGet("{patientId}/notes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<NoteResponseDto>>> GetNotesByPatient(int patientId)
        {
            var query = new GetAllNotesByPatientQuery(patientId);
            var notes = await _mediator.Send(query);

            return Ok(notes);
        }


        /// <summary>
        /// Actualiza una nota específica de un paciente.
        /// </summary>
        /// <param name="patientId">ID del paciente.</param>
        /// <param name="noteId">ID de la nota.</param>
        /// <param name="updateDto">Datos actualizados de la nota.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="204">Nota actualizada exitosamente.</response>
        /// <response code="400">Datos inválidos.</response>
        /// <response code="404">Nota no encontrada.</response>
        [HttpPut("{patientId}/notes/{noteId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateNote(int patientId, int noteId, [FromBody] UpdateNoteDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateNoteCommand
            {
                Id = noteId,
                Note = new UpdateNoteDto
                {
                    Title = updateDto.Title,
                    Content = updateDto.Content
                }
            };
            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound($"No se encontró la nota con ID {noteId} para el paciente {patientId}");


            return NoContent();
        }

        /// <summary>
        /// Elimina una nota específica de un paciente.
        /// </summary>
        /// <param name="patientId">ID del paciente.</param>
        /// <param name="noteId">ID de la nota.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="204">Nota eliminada exitosamente.</response>
        /// <response code="404">Nota no encontrada.</response>
        [HttpDelete("{patientId}/notes/{noteId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNote(int patientId, int noteId)
        {
            var command = new DeleteNoteCommand(noteId);
            var result = await _mediator.Send(command);

            if (result is null)
                return NotFound($"No se encontró la nota con ID {noteId} para el paciente {patientId}");

            return NoContent();
        }
    }
}
