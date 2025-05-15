using MediatR;
using ProyectoFoo.Application.Features.Notes;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes.Queries
{
    public class GetNotesByPatientQueryHandler : IRequestHandler<GetNotesByPatientQuery, List<NoteResponseDto>>
    {
        private readonly IPatientNoteRepository _noteRepository;

        public GetNotesByPatientQueryHandler(IPatientNoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<List<NoteResponseDto>> Handle(GetNotesByPatientQuery request, CancellationToken cancellationToken)
        {
            var notes = await _noteRepository.GetByPatientIdAsync(request.PatientId, cancellationToken);

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
