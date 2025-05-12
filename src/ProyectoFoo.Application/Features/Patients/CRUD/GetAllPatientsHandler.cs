using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var allPatients = await _pacienteRepository.GetAllAsync();
            var enabledPatients = allPatients.Where(p => p.IsEnabled).ToList();

            if (enabledPatients != null && enabledPatients.Count != 0)
            {
                var patientsDto = enabledPatients.Select(patientEntity =>
                {
                    // Crear una instancia de Paciente para poder usar sus métodos
                    var paciente = new Paciente
                    {
                        Id = patientEntity.Id,
                        Name = patientEntity.Name,
                        Surname = patientEntity.Surname,
                        Birthdate = patientEntity.Birthdate,
                        TypeOfIdentification = patientEntity.TypeOfIdentification,
                        Identification = patientEntity.Identification,
                        Sex = patientEntity.Sex,
                        Modality = patientEntity.Modality,
                        Email = patientEntity.Email ?? string.Empty,
                        Phone = patientEntity.Phone ?? string.Empty,
                        Nationality = patientEntity.Nationality,
                        AdmissionDate = patientEntity.AdmissionDate // Asegúrate de mapear AdmissionDate si la usas en los cálculos
                    };

                    // Calcular la edad y el rango usando los métodos de la entidad
                    paciente.Age = paciente.CalculateAge(paciente.Birthdate);
                    paciente.AgeRange = paciente.CalculateAgeRange(paciente.Age);

                    return new PatientDTO
                    {
                        Id = paciente.Id,
                        Name = paciente.Name.CapitalizeFirstLetter(),
                        Surname = paciente.Surname.CapitalizeFirstLetter(),
                        Birthdate = paciente.Birthdate,
                        TypeOfIdentification = paciente.TypeOfIdentification.ToUpper(),
                        Identification = paciente.Identification,
                        Sex = paciente.Sex,
                        Modality = paciente.Modality,
                        Email = paciente.Email,
                        Phone = paciente.Phone,
                        Age = paciente.Age,
                        RangoEtario = paciente.AgeRange, // Usar el valor calculado por la entidad
                        Nationality = paciente.Nationality.CapitalizeFirstLetter()
                    };
                }).ToList();

                return new GetAllPatientsResponse
                {
                    Patients = patientsDto,
                    Success = true
                };
            }
            else
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
