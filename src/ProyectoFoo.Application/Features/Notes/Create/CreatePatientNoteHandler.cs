using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes.Create
{
    public class CreatePatientNoteHandler : IRequestHandler<CreatePatientNoteCommand, CreatePatientNoteResponse>
    {
        private readonly IPatientNoteRepository _noteRepository;
        private readonly IPatientRepository _patientRepository;


        public CreatePatientNoteHandler(IPatientNoteRepository noteRepository, IPatientRepository patientRepository)
        {
            _noteRepository = noteRepository;
            _patientRepository = patientRepository;
        }

        public async Task<CreatePatientNoteResponse> Handle(CreatePatientNoteCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetByIdAsync(request.PatientId);

            try
            {
                if (patient == null)
                {
                    return new CreatePatientNoteResponse
                    {
                        Success = false,
                        Message = $"No se encontró el paciente con ID: {request.PatientId}"
                    };
                }


                var patientNoteEntity = new PatientNote
                {
                    PatientId = request.PatientId,
                    Title = request.Title,
                    Date = request.Date,
                    Content = request.Content,
                    CreationDate = DateTime.UtcNow
                };


                var newPatientNoteEntity = await _noteRepository.AddAsync(patientNoteEntity);
                if (newPatientNoteEntity == null)
                {
                    return new CreatePatientNoteResponse
                    {
                        Success = false,
                        Message = "No se pudo crear la nota del paciente."
                    };
                }
                else
                {
                    return new CreatePatientNoteResponse
                    {
                        Success = true,
                        Message = "Material del paciente creado exitosamente.",
                        PatientNoteId = newPatientNoteEntity.Id,
                        Note = new PatientNoteDto
                        {
                            Id = newPatientNoteEntity.Id,
                            PatientId = newPatientNoteEntity.PatientId,
                            Title = newPatientNoteEntity.Title,
                            Content = newPatientNoteEntity.Content ?? string.Empty,
                            CreationDate = newPatientNoteEntity.CreationDate
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                return new CreatePatientNoteResponse
                {
                    Success = false,
                    Message = "Hubo un error al crear la nota: " + ex.Message
                };
            }
        }
    }
}
