using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoFoo.API.Helpers;
using ProyectoFoo.Application.Features.Notes.Create;
using ProyectoFoo.Application.Features.Notes.Delete;
using ProyectoFoo.Application.Features.Notes.Read;
using ProyectoFoo.Application.Features.Notes.Update;
using ProyectoFoo.Application.Features.Patients.Search;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.API.Controllers
{
    /// <summary>
    /// Controlador API para manejar operaciones sobre notas de paciente.
    /// </summary>
    [Authorize] // Requiere autenticación para todas las acciones en este controlador
    [Route("api/[controller]")]
    [ApiController]
    public class PatientNoteController(IMediator mediator) : ControllerBase
    {

            private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));


        /// <summary>
        /// Crea una nueva nota para un paciente específico.
        /// </summary>
        /// <param name="patientId">ID del paciente.</param>
        /// <param name="note">Datos de la nota a crear.</param>
        /// <response code="404">Si el paciente especificado no existe.</response>
        /// <response code="500">Error interno del servidor.</response>
        /// <returns>La nota recién creada.</returns>
        /// <response code="201">Nota creada exitosamente.</response>
        /// <response code="400">Datos inválidos.</response>
        [HttpPost("{patientId}/notes")]
        [ProducesResponseType(typeof(PatientNoteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PatientNoteDto>> CreateNote(
             [FromRoute] int patientId,
             [FromBody] CreatePatientNoteCommand note)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                note.PatientId = patientId;

                var response = await _mediator.Send(note);

                if (response.Success)
                {
                    return CreatedAtAction(nameof(GetPatientNoteById),
                     new { patientId = response.Note.PatientId, noteId = response.Note.Id },
                    response.Note);
                }

                if (response.Message.Contains("No se encontró el paciente"))
                {
                    return NotFound(new ProblemDetails { Title = "Paciente no encontrado", Detail = response.Message, Status = StatusCodes.Status404NotFound });
                }

                return BadRequest(new ProblemDetails { Title = "Error al crear nota", Detail = response.Message ?? "No se pudo crear el nota." });
            }
            catch (Exception)
            {
                return Problem(detail: "Error inesperado al crear nota.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }
        }

        /// <summary>
        /// Obtiene una nota específica por su ID para un paciente específico.
        /// </summary>
        /// <param name="patientId">ID del paciente.</param>
        /// <param name="noteId">ID de la nota.</param>
        /// <returns>El <see cref="PatientNoteDto"/> solicitado.</returns>
        /// <response code="200">Retorna la nota solicitado.</response>
        /// <response code="404">Si el paciente o la nota no existen.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{patientId}/notes/{noteId}")]
        [ProducesResponseType(typeof(PatientNoteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientNoteDto>> GetPatientNoteById(
                [FromRoute] int patientId,
                [FromRoute] int noteId)
        {
            try
            {
                var query = new GetPatientNoteByIdQuery { PatientId = patientId, NoteId = noteId };
                var note = await _mediator.Send(query);

                if (note != null)
                {
                    return Ok(note);
                }

                return NotFound(new ProblemDetails
                {
                    Title = "Recurso no encontrado",
                    Detail = $"No se encontró nota con ID {noteId} para el paciente con ID {patientId}.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception)
            {
                return Problem(detail: "Error inesperado al obtener el material del paciente.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }
        }


        /// <summary>
        /// Obtiene todas las notas de un paciente específico.
        /// </summary>
        /// <param name="patientId">ID del paciente.</param>
        /// <returns>Lista de <see cref="PatientNoteDto"/> del paciente.</returns>
        /// <response code="200">Retorna la lista de notas del paciente. Puede ser una lista vacía si no tiene materiales.</response>
        /// <response code="404">Si el paciente especificado no existe.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{patientId}/notes")]
        [ProducesResponseType(typeof(List<PatientNoteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PatientNoteDto>>> GetAllPatientNotes(
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

                var query = new GetAllPatientNotesQuery { PatientId = patientId };
                var notes = await _mediator.Send(query);

                if (notes != null && notes.Count > 0)
                {
                    return Ok(notes);
                }

                return Ok(new List<PatientMaterialDto>());
            }
            catch (Exception)
            {
                return Problem(detail: "Error inesperado al obtener los materiales del paciente.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }
        }


        /// <summary>
        /// Actualiza la información de una nota específica de un paciente.
        /// </summary>
        /// <param name="patientId">ID del paciente.</param>
        /// <param name="noteId">ID de la nota.</param>
        /// <param name="updatePatientNoteDto">Datos actualizados de la nota.</param>
        /// <returns>Un <see cref="UpdatePatientNoteResponse"/> que indica el resultado.</returns>
        /// <response code="200">Retorna <see cref="UpdatePatientNoteResponse"/> con el material actualizado o mensaje de éxito.</response>
        /// <response code="400">Si la petición no es válida (ej. datos faltantes).</response>
        /// <response code="404">Si el paciente o la nota no existen.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{patientId}/notes/{noteId}")]
        [ProducesResponseType(typeof(UpdatePatientNoteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePatientNote(
       [FromRoute] int patientId,
        [FromRoute] int noteId,
        [FromBody] PatientNoteDto updatePatientNoteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {

                var command = new UpdatePatientNoteCommand
                {
                    PatientId = patientId,
                    NoteId = noteId,
                    Note = updatePatientNoteDto
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
                    return BadRequest(new ProblemDetails { Title = "Error al actualizar la nota", Detail = response.Message ?? "No se pudo actualizar la nota." });
                }
            }
            catch (Exception)
            {
                return Problem(detail: "Error inesperado al actualizar la nota del paciente.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }
        }

        /// <summary>
        /// Elimina una nota específica de un paciente.
        /// </summary>
        /// <param name="patientId">ID del paciente.</param>
        /// <param name="noteId">ID de la nota.</param>
        /// <returns>No devuelve contenido en caso de éxito.</returns>
        /// <response code="204">Si el material fue eliminado exitosamente.</response>
        /// <response code="401">Si el usuario no está autorizado.</response>
        /// <response code="404">Si el paciente o el material no existen.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{patientId}/notes/{noteId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePatientNote(
            [FromRoute] int patientId,
            [FromRoute] int noteId)
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


                var command = new DeletePatientNoteCommand
                {
                    PatientId = patientId,
                    NoteId = noteId
                };

                var result = await _mediator.Send(command);
                if (result.Success)
                {
                    return NoContent();
                }

                return NotFound(new ProblemDetails { Title = "Recurso no encontrado", Detail = result.Message ?? "No se pudo eliminar el material.", Status = StatusCodes.Status404NotFound });
            }
            catch (Exception)
            {
                return Problem(detail: "Error inesperado al eliminar el material del paciente.", statusCode: StatusCodes.Status500InternalServerError, title: "Error Interno");
            }
        }




    }
}
