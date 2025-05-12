using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Application.ServiceExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProyectoFoo.Domain.Common.Enums;
namespace ProyectoFoo.Application.Features.Patients.CRUD
{
    public class CreatePatientHandler : IRequestHandler<CreatePatientCommand, CreatePatientResponse>
    {
        private readonly IPatientRepository _pacienteRepository;

        public CreatePatientHandler(IPatientRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<CreatePatientResponse> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            var paciente = new Paciente
            {
                Name = request.Name.CapitalizeFirstLetter(),
                Surname = request.Surname.CapitalizeFirstLetter(),
                Birthdate = request.Birthdate,
                TypeOfIdentification = request.TypeOfIdentification.ToUpper(),                
                Identification = request.Identification,
                Sex = request.Sex,
                Email = request.Email,
                Phone = request.Phone,
                Nationality = request.Nationality.CapitalizeFirstLetter(),
                PrincipalMotive = request.PrincipalMotive,
                ActualSymptoms = request.ActualSymptoms,
                RecentEvents = request.RecentEvents,
                PreviousDiagnosis = request.PreviousDiagnosis,
                ProfesionalObservations = request.ProfesionalObservations,
                KeyWords = request.KeyWords,
                FailedActs = request.FailedActs,
                Interconsulation = request.Interconsulation,
                PatientEvolution = request.PatientEvolution,
                SessionDay = request.SessionDay,
                Modality = request.Modality,
                SessionDuration = request.SessionDuration,
                SessionFrequency = request.SessionFrequency,
                PreferedContact = request.PreferedContact,
            };

            paciente.AdmissionDate = DateTime.UtcNow;
            paciente.Age = paciente.CalculateAge(paciente.Birthdate);
            paciente.AgeRange = paciente.CalculateAgeRange(paciente.Age);

            try
            {
                var newPaciente = await _pacienteRepository.AddAsync(paciente);

                var patientDto = new PatientDTO
                {
                    Id = newPaciente.Id,
                    Name = newPaciente.Name,
                    Surname = newPaciente.Surname,
                    Birthdate = newPaciente.Birthdate,
                    TypeOfIdentification = newPaciente.TypeOfIdentification,
                    Identification = newPaciente.Identification,
                    Sex = newPaciente.Sex,
                    Modality = newPaciente.Modality,
                    Email = newPaciente.Email,
                    Phone = newPaciente.Phone,
                    Age = newPaciente.Age,
                    AdmissionDate = newPaciente.AdmissionDate,
                    Nationality = newPaciente.Nationality,
                    RangoEtario = newPaciente.AgeRange,
                    PrincipalMotive = newPaciente.PrincipalMotive,
                    ActualSymptoms = newPaciente.ActualSymptoms,
                    RecentEvents = newPaciente.RecentEvents,
                    PreviousDiagnosis = newPaciente.PreviousDiagnosis,
                    ProfesionalObservations = newPaciente.ProfesionalObservations,
                    KeyWords = newPaciente.KeyWords,
                    FailedActs = newPaciente.FailedActs,
                    Interconsulation = newPaciente.Interconsulation,
                    PatientEvolution = newPaciente.PatientEvolution,
                    SessionDay = newPaciente.SessionDay,
                    SessionDuration = newPaciente.SessionDuration,
                    SessionFrequency = newPaciente.SessionFrequency,
                    PreferedContact = newPaciente.PreferedContact
                };

                return new CreatePatientResponse
                {
                    PatientId = patientDto.Id,
                    PatientDto = patientDto,
                    Success = true,
                    Message = "Paciente creado exitosamente.",
                };
            }
            catch (Exception ex)
            {
                return new CreatePatientResponse
                {
                    Success = false,
                    Message = "Hubo un error al crear el paciente: " + ex.Message
                };
            }
        }
    }
}
