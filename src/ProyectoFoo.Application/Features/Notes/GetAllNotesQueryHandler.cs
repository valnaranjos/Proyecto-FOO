using MediatR;
using ProyectoFoo.Application.Common.Interfaces;
using ProyectoFoo.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes.Queries
{
    public class GetAllNotesQueryHandler : IRequestHandler<GetAllNotesQuery, List<NoteResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllNotesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NoteResponseDto>> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
        {
            var notes = _context.Notes
                .Select(note => new NoteResponseDto
                {
                    Id = note.Id,
                    Title = note.Title,
                    Content = note.Content,
                    CreatedDate = note.CreatedDate,
                    PatientId = note.PatientId
                })
                .ToList();

            return await Task.FromResult(notes);
        }
    }
}
