using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes.Handlers
{
    public class CreateNoteHandler : IRequestHandler<CreateNoteCommand, CreateNoteResponse>
    {
        private readonly IPatientNoteRepository _noteRepository;
        private readonly IPatientRepository _patientRepository;


        public CreateNoteHandler(IPatientNoteRepository noteRepository, IPatientRepository patientRepository)
        {
            _noteRepository = noteRepository;
            _patientRepository = patientRepository;
        }

        public async Task<CreateNoteResponse> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetByIdAsync(request.PatientId);
            if (patient == null)
            {
                return new CreateNoteResponse
                {
                    Success = false,
                    Message = $"No se encontró el paciente con ID: {request.PatientId}"
                };
            }

            if (string.IsNullOrWhiteSpace(request.Note.Content))
            {
                return new CreateNoteResponse
                {
                    Success = false,
                    Message = "La nota no puede estar vacía."
                };
            }

            var patientNoteEntity = new PatientNote
            {
                PatientId = request.PatientId,
                Content = request.Note.Content,
                CreatedDate = DateTime.UtcNow
            };

            try
            {
                var added = await _noteRepository.AddAsync(patientNoteEntity);
                var noteDto = new PatientNoteDto
                {
                    Id = added.Id,
                    Content = added.Content,
                    CreatedDate = added.CreatedDate
                };

                return new CreateNoteResponse
                {
                    PatientNote = noteDto,
                    Success = true,
                    Message = "Nota del paciente creada exitosamente."
                };
            }
            catch (Exception ex)
            {
                return new CreateNoteResponse
                {
                    Success = false,
                    Message = "Hubo un error al crear la nota: " + ex.Message
                };
            }
        }
    }
}
