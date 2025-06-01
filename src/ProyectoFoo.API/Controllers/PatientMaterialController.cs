using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProyectoFoo.API.Helpers;
using ProyectoFoo.Application.Features.PatientMaterials.Create;
using ProyectoFoo.Application.Features.PatientMaterials.Delete;
using ProyectoFoo.Application.Features.PatientMaterials.Read;
using ProyectoFoo.Application.Features.PatientMaterials.Update;
using ProyectoFoo.Application.Features.PatientMaterials;
using ProyectoFoo.Application.Features.Patients.Search;
using ProyectoFoo.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoFoo.API.Controllers
{

    /// <summary>
    /// Controlador API para manejar operaciones sobre materiales de paciente.
    /// </summary>
    [Authorize] // Requiere autenticación para todas las acciones en este controlador
    [Route("api/[controller]")]
    [ApiController]
    public class PatientMaterialController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));


        /// <summary>
        /// Crea un nuevo material para un paciente específico.
        /// </summary>
        /// <param name="patientId">Identificador único del paciente.</param>
        /// <param name="command">Datos para la creación del material.</param>
        /// <returns>Un ActionResult que indica el resultado de la creación.</returns>
        /// <response code="201">Retorna el material recién creado.</response>
        /// <response code="400">Si la petición no es válida.</response>
        /// <response code="404">Si el paciente especificado no existe.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("{patientId}/materials")]
        [ProducesResponseType(typeof(PatientMaterialDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PatientMaterialDto>> CreatePatientMaterial(
            [FromRoute] int patientId,
            [FromBody] CreatePatientMaterialCommand command)
        {

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                command.PatientId = patientId;

                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    return CreatedAtAction(nameof(GetPatientMaterialById),
                 new { patientId = response.Material.PatientId, MaterialId = response.Material.Id },
                response.Material);
                }

                if (response.Message.Contains("No se encontró el paciente"))
                {
                    return NotFound(new ProblemDetails { Title = "Paciente no encontrado", Detail = response.Message, Status = StatusCodes.Status404NotFound });
                }

                return BadRequest(new ProblemDetails { Title = "Error al crear material", Detail = response.Message ?? "No se pudo crear el material." });
            }
            catch (Exception)
            {
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
        public async Task<ActionResult<List<PatientMaterialDto>>> GetAllPatientMaterials(
            [FromRoute] int patientId)
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

                var patientExists = await _mediator.Send(new GetPatientByIdQuery(patientId, currentUserId.Value));
                if (patientExists == null)
                {
                    return NotFound($"No se encontró el paciente con ID: {patientId}");
                }

                var query = new GetAllPatientMaterialsQuery { PatientId = patientId };
                var response = await _mediator.Send(query);

                if (response != null && response.Count > 0)
                {
                    return Ok(response);
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
        public async Task<ActionResult<PatientMaterialDto>> GetPatientMaterialById(
            [FromRoute] int patientId,
            [FromRoute] int materialId)
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
        [FromBody] PatientMaterialDto updatePatientMaterialDto)
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
        /// <response code="401">Si el usuario no está autorizado.</response>
        /// <response code="404">Si el paciente o el material no existen.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{patientId}/materials/{materialId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePatientMaterial(
        [FromRoute] int patientId,
        [FromRoute] int materialId)
        {
            try
            {
                var currentUserId = this.GetCurrentUserId();

                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new ProblemDetails
                    {
                        Title = "No autorizado",
                        Detail = "El ID del usuario no pudo ser extraído del token de autenticación.",
                        Status = StatusCodes.Status401Unauthorized
                    });
                }

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


        /// <summary>
        /// Envía un material específico de un paciente por correo electrónico.
        /// </summary>
        /// <param name="patientId">Identificador único del paciente.</param>
        /// <param name="materialId">Identificador único del material a enviar.</param>
        /// <returns>Un ActionResult que indica el resultado del envío.</returns>
        /// <response code="200">El correo electrónico fue enviado exitosamente.</response>
        /// <response code="400">Si la petición no es válida o el paciente no tiene email.</response>
        /// <response code="401">Usuario no autenticado o ID de usuario no disponible.</response>
        /// <response code="404">Si el paciente o el material especificado no existe.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("{patientId}/materials/{materialId}/send-email")]
        [ProducesResponseType(typeof(SendPatientMaterialEmailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SendPatientMaterialEmailResponse>> SendPatientMaterialEmail(
            [FromRoute] int patientId,
            [FromRoute] int materialId)
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

            var command = new SendPatientMaterialEmailCommand
            {
                PatientId = patientId,
                MaterialId = materialId,
                UserId = currentUserId.Value,
            };

            var response = await _mediator.Send(command);

            if (response.Success)
            {
                return Ok(response);
            }

            if (response.Message.Contains("No se encontró el paciente") || response.Message.Contains("No se encontró el material"))
            {
                return NotFound(new ProblemDetails { Title = "Recurso no encontrado", Detail = response.Message });
            }
            if (response.Message.Contains("no tiene una dirección de correo electrónico"))
            {
                return BadRequest(new ProblemDetails { Title = "Email no disponible", Detail = response.Message });
            }

            return Problem(detail: response.Message, statusCode: StatusCodes.Status500InternalServerError, title: "Error al enviar email");
        }

    }
}
