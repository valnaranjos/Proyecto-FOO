using MediatR;
using ProyectoFoo.Application.Common.Interfaces;
using ProyectoFoo.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes.Queries
{
    public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, NoteResponseDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetNoteByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NoteResponseDto?> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
        {
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);

            if (note == null)
                return null;

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
