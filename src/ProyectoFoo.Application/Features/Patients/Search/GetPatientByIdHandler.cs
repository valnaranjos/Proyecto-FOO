using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.ServiceExtension;
using ProyectoFoo.Domain.Common.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Search
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
            try 
            { 

            var patient = await _pacienteRepository.GetByIdAndUserAsync(request.PatientId, request.UserId);

                if (patient == null)
                {
                    return new GetPatientByIdResponse
                    {
                        Success = false,
                        Message = "Paciente no encontrado."
                    };
                }

                if (patient.UserId != request.UserId)
                {
                    return new GetPatientByIdResponse
                    {
                        Success = false,
                        Message = "Paciente no encontrado o no tienes permiso para verlo."
                    };
                }


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
                    Interconsulation = patient.Interconsultation,
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
                    Success = true,
                };
            }
            catch (Exception)
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
