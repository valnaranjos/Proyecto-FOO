using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;

namespace ProyectoFoo.Application.Features.Notes.Handlers
{
    public class UpdateNoteHandler : IRequestHandler<UpdateNoteCommand, UpdateNoteResponse>
    {
        private readonly IPatientNoteRepository _noteRepository;

        public UpdateNoteHandler(IPatientNoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<UpdateNoteResponse> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
        {
            var existingNote = await _noteRepository.GetByIdAsync(request.Id);
            if (existingNote == null)
            {
                return new UpdateNoteResponse
                {
                    Success = false,
                    Message = $"No se encontró la nota con ID: {request.Id}"
                };
            }

            if (request.Note.Date.Date < DateTime.UtcNow.Date)
            {
                return new UpdateNoteResponse
                {
                    Success = false,
                    Message = "La fecha de la nota no puede ser anterior a la actual."
                };
            }

            existingNote.Title = request.Note.Title;
            existingNote.Content = request.Note.Content;
            existingNote.CreatedDate = request.Note.Date;

            try
            {
                _noteRepository.UpdateAsync(existingNote);

                return new UpdateNoteResponse
                {
                    Success = true,
                    Message = "Nota actualizada exitosamente."
                };
            }
            catch (Exception ex)
            {
                return new UpdateNoteResponse
                {
                    Success = false,
                    Message = $"Error al actualizar la nota: {ex.Message}"
                };
            }
        }
    }
}
