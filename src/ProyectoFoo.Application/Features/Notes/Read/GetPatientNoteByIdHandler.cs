using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes.Read
{
    public class GetPatientNoteByIdHandler : IRequestHandler<GetPatientNoteByIdQuery, PatientNoteDto?>
    {
        private readonly IPatientNoteRepository _noteRepository;
        private readonly IPatientRepository _patientRepository;

        public GetPatientNoteByIdHandler(IPatientNoteRepository noteRepository, IPatientRepository patientRepository)
        {
            _noteRepository = noteRepository ?? throw new ArgumentException(nameof(noteRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        }

        public async Task<PatientNoteDto?> Handle(GetPatientNoteByIdQuery request, CancellationToken cancellationToken)
        {
            var patientExists = await _patientRepository.GetByIdAsync(request.PatientId);
            if (patientExists == null)
            {
                return null;
            }

            var note = await _noteRepository.GetByIdAsync(request.NoteId);

            if (note == null || note.PatientId != request.PatientId)
                return null;

            return new PatientNoteDto
            {
                Id = note.Id,
                PatientId = note.PatientId,
                Title = note.Title,
                Content = note.Content ?? string.Empty
            };
        }
    }
}
