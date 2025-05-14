using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.PatientMaterials
{
    public class DeletePatientMaterialHandler : IRequestHandler<DeletePatientMaterialCommand, DeletePatientMaterialResponse>
    {
        private readonly IPatientMaterialRepository _patientMaterialRepository;
        private readonly IPatientRepository _patientRepository;

        public DeletePatientMaterialHandler(IPatientMaterialRepository patientMaterialRepository, IPatientRepository patientRepository)
        {
            _patientMaterialRepository = patientMaterialRepository ?? throw new ArgumentNullException(nameof(patientMaterialRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        }

        public async Task<DeletePatientMaterialResponse> Handle(DeletePatientMaterialCommand request, CancellationToken cancellationToken)
        {
            var response = new DeletePatientMaterialResponse();
            var patient = await _patientRepository.GetByIdAsync(request.PatientId);
            if (patient == null)
            {
                response.Success = false;
                response.Message = $"No se encontró el paciente con ID: {request.PatientId}";
                return response;
            }

            var existingMaterial = await _patientMaterialRepository.GetByIdAsync(request.MaterialId);
            if (existingMaterial == null || existingMaterial.PatientId != request.PatientId)
            {
                response.Success = false;
                response.Message = $"No se encontró el material con ID: {request.MaterialId} para el paciente con ID: {request.PatientId}";
                return response;
            }

            try
            {
                await _patientMaterialRepository.DeleteAsync(existingMaterial);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error al eliminar el material del paciente: {ex.Message}";
            }

            return response;
        }
    }
}
