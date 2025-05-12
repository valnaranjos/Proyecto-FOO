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
