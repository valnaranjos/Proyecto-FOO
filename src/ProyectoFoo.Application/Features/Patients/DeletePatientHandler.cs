using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients
{
    public class DeletePatientHandler : IRequestHandler<DeletePatientCommand, DeletePatientResponse>
    {
        private readonly IPatientRepository _pacienteRepository;

        public DeletePatientHandler(IPatientRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<DeletePatientResponse> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            var patientToDelete = await _pacienteRepository.GetByIdAsync(request.PatientId);

            if (patientToDelete == null)
            {
                return new DeletePatientResponse
                {
                    PatientId = request.PatientId,
                    Success = false,
                    Message = $"Paciente con ID {request.PatientId} no encontrado."
                };
            }

            await _pacienteRepository.DeleteAsync(patientToDelete);

            return new DeletePatientResponse
            {
                PatientId = request.PatientId,
                Success = true
            };
        }
    }
}
