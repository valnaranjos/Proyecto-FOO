using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.ServiceExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Archive
{
    public  class GetAllArchivedPatientsHandler(IPatientRepository patientRepository) : IRequestHandler<GetAllArchivedPatientsCommand, List<PatientDTO>>
    {
        private readonly IPatientRepository _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));

        public async Task<List<PatientDTO>> Handle(GetAllArchivedPatientsCommand request, CancellationToken cancellationToken)
        {
            var allPatients = await _patientRepository.GetPatientsByUserIdAsync(request.UserId);
            var disabledPatients = allPatients.Where(p => !p.IsEnabled).ToList();

            return disabledPatients.Select(patientEntity => new PatientDTO
            {
                Id = patientEntity.Id,
                Name = patientEntity.Name.CapitalizeFirstLetter(),
                Surname = patientEntity.Surname.CapitalizeFirstLetter(),
                Birthdate = patientEntity.Birthdate,
                TypeOfIdentification = patientEntity.TypeOfIdentification.ToUpper(),
                Identification = patientEntity.Identification,
                Sex = patientEntity.Sex,
                Modality = patientEntity.Modality,
                Email = patientEntity.Email ?? string.Empty,
                Phone = patientEntity.Phone ?? string.Empty,
                Age = patientEntity.Age,
                AdmissionDate = patientEntity.AdmissionDate,
                RangoEtario = patientEntity.AgeRange,
                Nationality = patientEntity.Nationality.CapitalizeFirstLetter(),
                PrincipalMotive = patientEntity.PrincipalMotive,
                ActualSymptoms = patientEntity.ActualSymptoms,
                RecentEvents = patientEntity.RecentEvents,
                PreviousDiagnosis = patientEntity.PreviousDiagnosis,
                ProfesionalObservations = patientEntity.ProfesionalObservations,
                KeyWords = patientEntity.KeyWords,
                FailedActs = patientEntity.FailedActs,
                Interconsulation = patientEntity.Interconsulation,
                PatientEvolution = patientEntity.PatientEvolution,
                SessionDay = patientEntity.SessionDay,
                SessionDuration = patientEntity.SessionDuration,
                SessionFrequency = patientEntity.SessionFrequency,
                PreferedContact = patientEntity.PreferedContact,
            }).ToList();
        }
    }
}
