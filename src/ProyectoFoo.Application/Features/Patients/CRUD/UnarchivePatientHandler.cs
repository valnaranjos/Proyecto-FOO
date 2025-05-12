using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.CRUD
{
    public class UnarchivePatientHandler : IRequestHandler<UnarchivePatientCommand, UnarchivePatientResponse>
    {
        private readonly IPatientRepository _patientRepository;

        public UnarchivePatientHandler(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        }

        public async Task<UnarchivePatientResponse> Handle(UnarchivePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetByIdAsync(request.PatientId);

            if (patient == null)
            {
                return new UnarchivePatientResponse { Success = false, Message = $"Paciente con ID {request.PatientId} no encontrado." };
            }

            patient.IsEnabled = true;

            try
            {
                await _patientRepository.UpdateAsync(patient);
                return new UnarchivePatientResponse { Success = true, Message = $"Paciente con ID {request.PatientId} ha sido desarchivado." };
            }
            catch (Exception ex)
            {
                return new UnarchivePatientResponse { Success = false, Message = $"Error al desarchivar el paciente con ID {request.PatientId}: {ex.Message}" };
            }
        }
    }
}
