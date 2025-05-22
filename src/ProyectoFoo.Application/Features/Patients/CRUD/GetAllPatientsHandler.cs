using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Common.Enums;
using ProyectoFoo.Application.ServiceExtension;
using ProyectoFoo.Domain.Entities;



namespace ProyectoFoo.Application.Features.Patients.CRUD
{
    public class GetAllPatientsHandler : IRequestHandler<GetAllPatientsQuery, GetAllPatientsResponse>
    {
        private readonly IPatientRepository _pacienteRepository;

        public GetAllPatientsHandler(IPatientRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<GetAllPatientsResponse> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var allPatients = await _pacienteRepository.GetPatientsByUserIdAsync(request.UserId);
            
                var patientsDto = allPatients.Select(patient  => new PatientDTO
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
                }).ToList();

                return new GetAllPatientsResponse
                {
                    Patients = patientsDto,
                    Success = true
                };
            }
            catch (Exception)
            {
                return new GetAllPatientsResponse
                {
                    Patients = [], // Devuelve una lista vacía si no hay pacientes
                    Success = true,
                    Message = "No se encontraron pacientes."
                };
            }
        }
    }
}
