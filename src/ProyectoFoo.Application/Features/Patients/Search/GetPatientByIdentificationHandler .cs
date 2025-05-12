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
                    Nationality = patient.Nationality.CapitalizeFirstLetter(),
                    TypeOfIdentification = patient.TypeOfIdentification.ToUpper(),
                    Identification = patient.Identification,
                    Sex = patient.Sex,
                    Email = patient.Email,
                    Phone = patient.Phone,
                    AdmissionDate = patient.AdmissionDate,
                    Age = patient.CalculateAge(patient.Birthdate),
                    RangoEtario = patient.CalculateAgeRange(patient.Age),

                    PrincipalMotive = patient.PrincipalMotive,
                    ActualSymptoms = patient.ActualSymptoms,
                    RecentEvents = patient.RecentEvents,
                    PreviousDiagnosis = patient.PreviousDiagnosis,
                    ProfesionalObservations = patient.ProfesionalObservations,
                    KeyWords = patient.KeyWords,
                    FailedActs = patient.FailedActs,
                    Interconsulation = patient.Interconsulation,
                    PatientEvolution = patient.PatientEvolution,
                    SessionDay = patient.SessionDay,
                    Modality = patient.Modality,
                    SessionDuration = patient.SessionDuration,
                    SessionFrequency = patient.SessionFrequency,
                    PreferedContact = patient.PreferedContact,
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