using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.PatientMaterials
{
    public class UpdatePatientMaterialHandler : IRequestHandler<UpdatePatientMaterialCommand, UpdatePatientMaterialResponse>
    {
        private readonly IPatientMaterialRepository _patientMaterialRepository;
        private readonly IPatientRepository _patientRepository;


        public UpdatePatientMaterialHandler(IPatientMaterialRepository patientMaterialRepository, IPatientRepository patientRepository)
        {
            _patientMaterialRepository = patientMaterialRepository;
            _patientRepository = patientRepository;
        }

        public async Task<UpdatePatientMaterialResponse> Handle(UpdatePatientMaterialCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdatePatientMaterialResponse();
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

            if (request.Material.Date.Date < DateTime.UtcNow.Date)
            {
                response.Success = false;
                response.Message = "La fecha de la sesión no puede ser anterior a la fecha actual.";
                return response;
            }

            // Actualizar solo si se proporciona un nuevo valor
            if (request.Material.Title != null)
            {
                existingMaterial.Title = request.Material.Title;
            }

            // Para DateTime, puedes verificar si es diferente del valor por defecto (DateTime.MinValue)
            if (request.Material.Date != default(DateTime))
            {
                // Validar la fecha de la sesión (no anterior a la actual, si aplica)
                if (request.Material.Date.Date >= DateTime.UtcNow.Date)
                {
                }
                else
                {
                    response.Success = false;
                    response.Message = "La fecha de la sesión no puede ser anterior a la fecha actual.";
                    return response;
                }
                existingMaterial.Date = request.Material.Date;
            }

            if (request.Material.Content != null)
            {
                existingMaterial.Content = request.Material.Content;
            }


            try
            {
                await _patientMaterialRepository.UpdateAsync(existingMaterial);

                response.Success = true;
                response.PatientMaterial = new PatientMaterialDto
                {
                    Id = existingMaterial.Id,
                    Title = existingMaterial.Title,
                    Date = existingMaterial.Date,
                    Content = existingMaterial.Content
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error al actualizar el material del paciente: {ex.Message}";
            }

            return response;
        }
    }
}

