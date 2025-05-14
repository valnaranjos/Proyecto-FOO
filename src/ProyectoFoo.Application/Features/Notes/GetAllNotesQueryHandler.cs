using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Shared.Models;
using ProyectoFoo.Application.Features.Notes.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes.Handlers
{
    public class GetAllNotesQueryHandler : IRequestHandler<GetAllNotesQuery, List<NoteResponseDto>>
    {
        private readonly IPatientNoteRepository _noteRepository;

        public GetAllNotesQueryHandler(IPatientNoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<List<NoteResponseDto>> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
        {
            var notes = await _noteRepository.GetAllAsync(cancellationToken);

            return notes.Select(note => new NoteResponseDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedDate = note.CreatedDate,
                PatientId = note.PatientId
            }).ToList();
        }
    }
}
