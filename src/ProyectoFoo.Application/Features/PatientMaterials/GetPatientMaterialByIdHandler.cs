using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.PatientMaterials
{
    public class GetPatientMaterialByIdHandler : IRequestHandler<GetPatientMaterialByIdQuery, PatientMaterialDto>
    {
        private readonly IPatientMaterialRepository _patientMaterialRepository;
        private readonly IPatientRepository _patientRepository; // Para verificar que el paciente existe

        public GetPatientMaterialByIdHandler(IPatientMaterialRepository pacienteMaterialRepository, IPatientRepository pacienteRepository)
        {
            _patientMaterialRepository = pacienteMaterialRepository ?? throw new ArgumentNullException(nameof(pacienteMaterialRepository));
            _patientRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<PatientMaterialDto> Handle(GetPatientMaterialByIdQuery request, CancellationToken cancellationToken)
        {
            // Verificar si el paciente existe
            var patientExists = await _patientRepository.GetByIdAsync(request.PatientId);
            if (patientExists == null)
            {
                return null; 
            }

            // Obtener el material por su ID
            var material = await _patientMaterialRepository.GetByIdAsync(request.MaterialId);

            // Verificar si el material existe y pertenece al paciente especificado
            if (material == null || material.PatientId != request.PatientId)
            {
                return null; // O podrías lanzar una excepción o devolver un DTO nulo con un mensaje
            }

            // Mapear la entidad a DTO
            var materialDto = new PatientMaterialDto
            {
                Id = material.Id,
                Title = material.Title,
                Date = material.Date,
                Content = material.Content
            };

            return materialDto;
        }
    }
}