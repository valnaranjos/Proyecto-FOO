using MediatR;
using Microsoft.EntityFrameworkCore;
using ProyectoFoo.Application.Common.Interfaces;
using ProyectoFoo.Application.Features.Notes;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes.Handlers
{
    public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, NoteResponseDto>
    {
        private readonly IApplicationDbContext _context;

        public UpdateNoteCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NoteResponseDto> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
        {
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);

            if (note == null)
                throw new KeyNotFoundException($"Note with ID {request.Id} not found.");

            note.Title = request.Note.Title;
            note.Content = request.Note.Content;

            await _context.SaveChangesAsync(cancellationToken);

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
