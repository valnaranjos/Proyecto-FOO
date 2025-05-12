using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.ServiceExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Search
{
    public class GetPatientsByNationalityHandler : IRequestHandler<GetPatientsByNationalityCommand, GetPatientsByNationalityResponse>
    {
        private readonly IPatientRepository _pacienteRepository;

        public GetPatientsByNationalityHandler(IPatientRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<GetPatientsByNationalityResponse> Handle(GetPatientsByNationalityCommand request, CancellationToken cancellationToken)
        {
            var patients = await _pacienteRepository.GetByNationalityAsync(request.Nationality);

            if (patients != null && patients.Any())
            {
                var patientsDto = patients.Select(patient => new PatientDTO
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

                return new GetPatientsByNationalityResponse
                {
                    Patients = patientsDto,
                    Success = true
                };
            }
            else
            {
                return new GetPatientsByNationalityResponse
                {
                    Success = false,
                    Message = $"No se encontraron pacientes con la nacionalidad: {request.Nationality}",
                    Patients = new List<PatientDTO>()
                };
            }
        }
    }
}
