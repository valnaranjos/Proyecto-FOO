using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.ServiceExtension;
using ProyectoFoo.Domain.Common.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients
{
    public class GetPatientByIdHandler : IRequestHandler<GetPatientByIdQuery, GetPatientByIdResponse>
    {
        private readonly IPatientRepository _pacienteRepository;

        public GetPatientByIdHandler(IPatientRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<GetPatientByIdResponse> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            var patient = await _pacienteRepository.GetByIdAsync(request.PatientId);

            if (patient != null)
            {
                var patientDto = new PatientDTO
                {
                    Id = patient.Id,
                    Name = patient.Name.CapitalizeFirstLetter(),
                    Surname = patient.Surname.CapitalizeFirstLetter(),
                    Birthdate = patient.Birthdate,
                    Identification = patient.Identification,

                    TypeOfIdentification = patient.TypeOfIdentification.ToUpper() ?? string.Empty,
                    Sex = patient.Sex,
                    Modality = patient.Modality,
                    Email = patient.Email ?? string.Empty,
                    Phone = patient.Phone ?? string.Empty,
                    Age = patient.CalculateAge(patient.Birthdate),
                    RangoEtario = patient.CalculateAgeRange(patient.Age),
                    Nationality = patient.Nationality.CapitalizeFirstLetter(),
                    AdmissionDate = patient.AdmissionDate,
                };

                return new GetPatientByIdResponse
                {
                    Patient = patientDto,
                    Success = true,
                };
            }
            else
            {
                return new GetPatientByIdResponse
                {
                    Success = false,
                    Message = $"Paciente con ID {request.PatientId} no encontrado."
                };
            }
        }
    }
}
