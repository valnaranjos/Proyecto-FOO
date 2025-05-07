using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients
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

            if (allPatients != null && allPatients.Any())
            {
                var patientsDto = allPatients.Select(patient => new PatientDTO
                {
                    Id = patient.Id,
                    Name = patient.Name,
                    Surname = patient.Surname,
                    Birthdate = patient.Birthdate,
                    Identification = patient.Identification,
                    Sex = patient.Sex.ToString(),
                    Modality = patient.Modality,
                    Email = patient.Email ?? string.Empty,
                    Phone = patient.Phone ?? string.Empty,
                    // Mapea aquí otras propiedades que necesites
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
                    Patients = new List<PatientDTO>(), // Devuelve una lista vacía si no hay pacientes
                    Success = true,
                    Message = "No se encontraron pacientes."
                };
            }
        }
    }
}
