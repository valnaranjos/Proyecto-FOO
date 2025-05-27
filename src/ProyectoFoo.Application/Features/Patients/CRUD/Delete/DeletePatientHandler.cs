using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.CRUD.Delete
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
            try
            {
                var patientToDelete = await _pacienteRepository.GetByIdAndUserAsync(request.PatientId, request.UserId);

                if (patientToDelete == null)
                {
                    return new DeletePatientResponse
                    {
                        PatientId = request.PatientId,
                        Success = false,
                        Message = $"Paciente con ID {request.PatientId} no encontrado."
                    };
                }

                if (patientToDelete.UserId != request.UserId)
                {
                    return new DeletePatientResponse
                    {
                        Success = false,
                        Message = "No tienes permiso para eliminar este paciente."
                    };
                }

                await _pacienteRepository.DeleteAsync(patientToDelete);

                return new DeletePatientResponse
                {
                    PatientId = request.PatientId,
                    Success = true
                };
            }
            catch (Exception ex)
            {

                return new DeletePatientResponse
                {
                    Success = false,
                    Message = $"Error al eliminar el paciente: {ex.Message}"
                };
            }
        }
    }
}
