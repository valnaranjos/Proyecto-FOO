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
    public class GetPatientMaterialByIdHandler(IPatientMaterialRepository pacienteMaterialRepository, IPatientRepository pacienteRepository) : IRequestHandler<GetPatientMaterialByIdQuery, PatientMaterialDto>
    {
        private readonly IPatientMaterialRepository _patientMaterialRepository = pacienteMaterialRepository ?? throw new ArgumentNullException(nameof(pacienteMaterialRepository));
        private readonly IPatientRepository _patientRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));

        public async Task<PatientMaterialDto> Handle(GetPatientMaterialByIdQuery request, CancellationToken cancellationToken)
        {
            var patientExists = await _patientRepository.GetByIdAsync(request.PatientId);
            if (patientExists == null)
            {
                return null; 
            }

            var material = await _patientMaterialRepository.GetByIdAsync(request.MaterialId);

          
            if (material == null || material.PatientId != request.PatientId)
            {
                return null;
            }

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