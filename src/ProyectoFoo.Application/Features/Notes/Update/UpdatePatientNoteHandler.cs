using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes.Update
{
    public class UpdatePatientNoteHandler : IRequestHandler<UpdatePatientNoteCommand, UpdatePatientNoteResponse>
    {
        private readonly IPatientNoteRepository _noteRepository;
        private readonly IPatientRepository _patientRepository;

        public UpdatePatientNoteHandler(IPatientNoteRepository noteRepository, IPatientRepository patientRepository)
        {
            _noteRepository = noteRepository;
            _patientRepository = patientRepository;
        }

        public async Task<UpdatePatientNoteResponse> Handle(UpdatePatientNoteCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdatePatientNoteResponse();

            if (request.Note == null)
            {
                response.Success = false;
                response.Message = "Datos de actualización de material no proporcionados.";
                return response;
            }

            var patient = await _patientRepository.GetByIdAsync(request.PatientId);
            if (patient == null)
            {
                response.Success = false;
                response.Message = $"No se encontró el paciente con ID: {request.PatientId}";
                return response;
            }

            var existingNote = await _noteRepository.GetByIdAsync(request.NoteId);
            if (existingNote == null)
            {
                return new UpdatePatientNoteResponse
                {
                    Success = false,
                    Message = $"No se encontró la nota con ID: {request.NoteId}"
                };
            }

            if (request.Note.Title != null)
            {
                existingNote.Title = request.Note.Title;
            }

            if (request.Note.Title != null)
            {
                existingNote.Content = request.Note.Content;
            }

            if (request?.ToString()?.Contains("SessionDate") == true)
            {
                existingNote.Date = request.Note.SessionDate;
               
            }

            try
            {
                await _noteRepository.UpdateAsync(existingNote);

                return new UpdatePatientNoteResponse
                {
                    Success = true,
                    Message = "Nota actualizada exitosamente.",
                    PatientNote = new PatientNoteDto
                    {
                        Id = existingNote.Id,
                        PatientId = existingNote.PatientId,
                        Title = existingNote.Title,
                        Content = existingNote.Content ?? string.Empty,
                        SessionDate = existingNote.Date
                    }
                };
            }
            catch (Exception ex)
            {
                return new UpdatePatientNoteResponse
                {
                    Success = false,
                    Message = $"Error al actualizar la nota: {ex.Message}"
                };
            }
        }
    }
}
