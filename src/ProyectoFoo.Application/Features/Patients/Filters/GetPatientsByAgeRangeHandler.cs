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

            if (patients != null && patients.Any())
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
                    RangoEtario = patient.AgeRange, // Ya es string en la entidad
                    Nationality = patient.Nationality.CapitalizeFirstLetter(),
                    AdmissionDate = patient.AdmissionDate,
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
