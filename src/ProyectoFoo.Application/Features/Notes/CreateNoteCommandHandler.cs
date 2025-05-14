using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes.Handlers
{
    public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, NoteResponseDto>
    {
        private readonly IPatientNoteRepository _noteRepository;

        public CreateNoteCommandHandler(IPatientNoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<NoteResponseDto> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
        {
            var note = new Note
            {
                Title = request.Note.Title,
                Content = request.Note.Content,
                CreatedDate = DateTime.UtcNow,
                PatientId = request.Note.PatientId
            };

            await _noteRepository.AddAsync(note, cancellationToken);
            await _noteRepository.SaveChangesAsync(cancellationToken);

            return new NoteResponseDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedDate = note.CreatedDate,
                PatientId = note.PatientId
            };
        }
    }
}
