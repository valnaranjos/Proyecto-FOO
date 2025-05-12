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
                    Identification = patient.Identification,
                    TypeOfIdentification = patient.TypeOfIdentification?.ToUpper() ?? string.Empty,
                    Sex = patient.Sex,
                    Modality = patient.Modality,
                    Email = patient.Email ?? string.Empty,
                    Phone = patient.Phone ?? string.Empty,
                    Age = patient.CalculateAge(patient.Birthdate),
                    RangoEtario = patient.CalculateAgeRange(patient.Age),
                    Nationality = patient.Nationality.CapitalizeFirstLetter(),
                    AdmissionDate = patient.AdmissionDate,
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
