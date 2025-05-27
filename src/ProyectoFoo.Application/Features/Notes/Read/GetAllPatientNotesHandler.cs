using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Shared.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes.Read
{
    public class GetAllPatientNotesHandler(IPatientNoteRepository noteRepository, IPatientRepository patientRepository) : IRequestHandler<GetAllPatientNotesQuery, List<PatientNoteDto>>
    {
        private readonly IPatientNoteRepository _noteRepository = noteRepository ?? throw new ArgumentNullException(nameof(noteRepository));
        private readonly IPatientRepository _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));

        public async Task<List<PatientNoteDto>> Handle(GetAllPatientNotesQuery request, CancellationToken cancellationToken)
        {
            var patientExists = await _patientRepository.GetByIdAsync(request.PatientId);
            if (patientExists == null)
            {
                return [];

            }
            var notes = await _noteRepository.GetByPatientIdAsync(request.PatientId);

            return [.. notes.Select(note => new PatientNoteDto
            {
                Id = note.Id,
                PatientId = note.PatientId,
                Title = note.Title,
                Content = note.Content ?? string.Empty,
                CreationDate = note.CreationDate
            })];
        }
    }
}
