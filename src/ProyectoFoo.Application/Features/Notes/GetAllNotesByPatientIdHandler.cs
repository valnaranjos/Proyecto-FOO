using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Shared.Models;
using ProyectoFoo.Application.Features.Notes.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes.Handlers
{
    public class GetAllNotesByPatientIdHandler : IRequestHandler<GetAllNotesByPatientQuery, List<PatientNoteDto>>
    {
        private readonly IPatientNoteRepository _noteRepository;

        public GetAllNotesByPatientIdHandler(IPatientNoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<List<PatientNoteDto>> Handle(GetAllNotesByPatientQuery request, CancellationToken cancellationToken)
        {
            var notes = await _noteRepository.GetNotesByPatientIdAsync(request.PatientId);

            if (notes == null || !notes.Any())
                return new List<PatientNoteDto>();

            return notes.Select(note => new PatientNoteDto
            {
                Id = note.Id,
                PatientId = note.PatientId,
                Title = note.Title,
                Content = note.Content,
                CreatedDate = note.CreatedDate
            }).ToList();
        }
    }
}
