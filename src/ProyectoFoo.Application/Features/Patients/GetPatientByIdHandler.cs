using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients
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
            var patient = await _pacienteRepository.GetByIdAsync(request.PatientId);

            if (patient != null)
            {
            SexType parsedSex;
            bool success = Enum.TryParse<SexType>(patient.Sex, out parsedSex);
            
            var patientDto = new PatientDTO
            {
               Id = patient.Id,
               Name = patient.Name,
               Surname = patient.Surname,
               Birthdate = patient.Birthdate,
               Identification = patient.Identification,
               Sex = success ? parsedSex : SexType.Otros,
               Modality = patient.Modality,
               Email = patient.Email ?? string.Empty,
               Phone = patient.Phone ?? string.Empty,
            };

                return new GetPatientByIdResponse
                {
                    Patient = patientDto,
                    Success = true,
                };
            }
            else
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
