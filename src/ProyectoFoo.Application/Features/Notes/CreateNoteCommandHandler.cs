using MediatR;
using ProyectoFoo.Application.Features.Notes;
using ProyectoFoo.Shared.Models;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes.Handlers
{
    public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, NoteResponseDto>
    {
        private readonly IApplicationDbContext _context;

        public CreateNoteCommandHandler(IApplicationDbContext context)
        {
            _context = context;
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

            _context.Notes.Add(note);
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
