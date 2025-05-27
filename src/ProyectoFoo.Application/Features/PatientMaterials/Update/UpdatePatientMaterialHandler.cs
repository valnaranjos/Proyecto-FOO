using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Shared.Models;
using ProyectoFoo.Shared.Models.PatientMaterial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.PatientMaterials.Update
{
    public class UpdatePatientMaterialHandler(IPatientMaterialRepository patientMaterialRepository, IPatientRepository patientRepository) : IRequestHandler<UpdatePatientMaterialCommand, UpdatePatientMaterialResponse>
    {
        private readonly IPatientMaterialRepository _patientMaterialRepository = patientMaterialRepository;
        private readonly IPatientRepository _patientRepository = patientRepository;

        public async Task<UpdatePatientMaterialResponse> Handle(UpdatePatientMaterialCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdatePatientMaterialResponse();
            if (request.Material == null)
            {
                response.Success = false;
                response.Message = "Datos de actualización de material no proporcionados.";
                return response;
            }

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

            /*
            if (request.Material.SessionDate.Date < DateTime.UtcNow.Date)
            {
                response.Success = false;
                response.Message = "La fecha de la sesión no puede ser anterior a la fecha actual.";
                return response;
            }*/

            if (request.Material.Title != null)
            {
                existingMaterial.Title = request.Material.Title;
            }

            /*
            // Para DateTime, puedes verificar si es diferente del valor por defecto (DateTime.MinValue)
            if (request.Material.SessionDate != default(DateTime))
            {
                // Validar la fecha de la sesión (no anterior a la actual, si aplica)
                if (request.Material.SessionDate.Date >= DateTime.UtcNow.Date)
                {
                }
                else
                {
                    response.Success = false;
                    response.Message = "La fecha de la sesión no puede ser anterior a la fecha actual.";
                    return response;
                }
                existingMaterial.Date = request.Material.SessionDate;
            }*/

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
                    SessionDate = existingMaterial.Date,
                    Content = existingMaterial.Content ?? string.Empty
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

