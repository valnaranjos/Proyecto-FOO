using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.Features.Patients;
using ProyectoFoo.Application.ServiceExtension;
using ProyectoFoo.Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Filters
{
    public class GetPatientsBySexTypeHandler : IRequestHandler<GetPatientsBySexTypeCommand, GetPatientsBySexTypeResponse>
    {
        private readonly IPatientRepository _pacienteRepository;

        public GetPatientsBySexTypeHandler(IPatientRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<GetPatientsBySexTypeResponse> Handle(GetPatientsBySexTypeCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<SexType>(request.Sex, true, out var sexType)) // Intentar convertir la cadena a SexType (ignorar mayúsculas/minúsculas)
            {
                return new GetPatientsBySexTypeResponse
                {
                    Success = false,
                    Message = $"El valor de sexo '{request.Sex}' no es válido."
                };
            }

            var patients = await _pacienteRepository.GetBySexTypeAsync(sexType);

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

                return new GetPatientsBySexTypeResponse
                {
                    Patients = patientsDto,
                    Success = true
                };
            }
            else
            {
                return new GetPatientsBySexTypeResponse
                {
                    Success = false,
                    Message = $"No se encontraron pacientes con el sexo: {request.Sex}",
                    Patients = []
                };
            }
        }
    }
}
