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
    public class GetNoteByIdHandler : IRequestHandler<GetNoteByIdQuery, PatientNoteDto?>
    {
        private readonly IPatientNoteRepository _noteRepository;

        public GetNoteByIdHandler(IPatientNoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<PatientNoteDto?> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken);

            if (note == null)
                return null;

            return new PatientNoteDto
            {
                Id = note.Id,
                PatientId = note.PatientId,
                Title = note.Title,
                Content = note.Content,
                CreatedDate = note.CreatedDate
            };
        }
    }
}
