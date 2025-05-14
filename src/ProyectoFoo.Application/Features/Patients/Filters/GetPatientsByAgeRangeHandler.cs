using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.ServiceExtension;
using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Application.Features.Patients.Filters
{
    public class GetPatientsByAgeRangeHandler : IRequestHandler<GetPatientsByAgeRangeCommand, GetPatientsByAgeRangeResponse>
    {
        private readonly IPatientRepository _pacienteRepository;

        public GetPatientsByAgeRangeHandler(IPatientRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<GetPatientsByAgeRangeResponse> Handle(GetPatientsByAgeRangeCommand request, CancellationToken cancellationToken)
        {
            var patients = await _pacienteRepository.GetByAgeRangeAsync(request.AgeRange);

            if (patients != null && patients.Count != 0)
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

                return new GetPatientsByAgeRangeResponse
                {
                    Patients = patientsDto,
                    Success = true
                };
            }
            else
            {
                return new GetPatientsByAgeRangeResponse
                {
                    Success = false,
                    Message = $"No se encontraron pacientes en el rango etario: {request.AgeRange}",
                    Patients = new List<PatientDTO>()
                };
            }
        }
    }
}
