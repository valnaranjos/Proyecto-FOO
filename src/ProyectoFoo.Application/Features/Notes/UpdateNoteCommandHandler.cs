using MediatR;
using ProyectoFoo.Application.Common.Interfaces.Repositories;
using ProyectoFoo.Application.Features.Notes;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes.Handlers
{
    public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, NoteResponseDto>
    {
        private readonly IPatientNoteRepository _noteRepository;

        public UpdateNoteCommandHandler(IPatientNoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<NoteResponseDto> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.GetByIdAsync(request.Id, cancellationToken);

            if (note == null)
                throw new KeyNotFoundException($"Note with ID {request.Id} not found.");

            note.Title = request.Note.Title;
            note.Content = request.Note.Content;

            await _noteRepository.SaveChangesAsync(cancellationToken);

            return new NoteResponseDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedDate = note.CreatedDate,
                PacienteId = note.PatientId
            };
        }
    }
}