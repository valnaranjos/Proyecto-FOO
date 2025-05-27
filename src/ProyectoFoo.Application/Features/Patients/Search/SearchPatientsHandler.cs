using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.ServiceExtension;

namespace ProyectoFoo.Application.Features.Patients.Search
{
    public class SearchPatientsHandler(IPatientRepository repo)
          : IRequestHandler<SearchPatientsQuery, IEnumerable<PatientDTO>>
    {
        private readonly IPatientRepository _patientRepository = repo;

        public async Task<IEnumerable<PatientDTO>> Handle(
            SearchPatientsQuery request,
            CancellationToken cancellationToken
        )
        {
            var pacientes = await _patientRepository.SearchAsync(
                request.Identification,
                request.FullName,
                request.Email,
                request.Nationality,
                request.SexType,
                request.Modality,
                request.AgeRange
            );

            return pacientes.Select(patient => new PatientDTO
            {
                Id = patient.Id,
                Identification = patient.Identification,
                FullName = $"{patient.Name} {patient.Surname}",
                Sex = patient.Sex,
                Modality = patient.Modality,
                Age = patient.Age,
                Birthdate = patient.Birthdate,
                TypeOfIdentification = patient.TypeOfIdentification.ToUpper(),
                Email = patient.Email ?? string.Empty,
                Phone = patient.Phone ?? string.Empty,
                AdmissionDate = patient.AdmissionDate,
                RangoEtario = patient.AgeRange,
                Nationality = patient.Nationality.CapitalizeFirstLetter(),
                PrincipalMotive = patient.PrincipalMotive,
                ActualSymptoms = patient.ActualSymptoms,
                RecentEvents = patient.RecentEvents,
                PreviousDiagnosis = patient.PreviousDiagnosis,
                ProfesionalObservations = patient.ProfesionalObservations,
                KeyWords = patient.KeyWords,
                FailedActs = patient.FailedActs,
                Interconsulation = patient.Interconsultation,
                PatientEvolution = patient.PatientEvolution,
                SessionDay = patient.SessionDay,
                SessionDuration = patient.SessionDuration,
                SessionFrequency = patient.SessionFrequency,
                PreferedContact = patient.PreferedContact,
            });
        }
    }
}
