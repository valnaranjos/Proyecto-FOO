using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using System.Threading;
using System.Threading.Tasks;
using ProyectoFoo.Application.ServiceExtension;

namespace ProyectoFoo.Application.Features.Patients.Search
{
    public class GetPatientByIdentificationHandler : IRequestHandler<GetPatientByIdentificationCommand, GetPatientByIdResponse>
    {
        private readonly IPatientRepository _pacienteRepository;

        public GetPatientByIdentificationHandler(IPatientRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<GetPatientByIdResponse> Handle(GetPatientByIdentificationCommand request, CancellationToken cancellationToken)
        {
            var patient = await _pacienteRepository.GetByIdentificationAsync(request.Identification);

            if (patient != null)
            {
                var patientDto = new PatientDTO
                {
                    Id = patient.Id,
                    Name = patient.Name.CapitalizeFirstLetter(),
                    Surname = patient.Surname.CapitalizeFirstLetter(),
                    Birthdate = patient.Birthdate,
                    Identification = patient.Identification,
                    TypeOfIdentification = patient.TypeOfIdentification?.ToUpper() ?? string.Empty,
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
                    Success = true
                };
            }
            else
            {
                return new GetPatientByIdResponse
                {
                    Success = false,
                    Message = $"No se encontró ningún paciente con la identificación: {request.Identification}"
                };
            }
        }
    }
}