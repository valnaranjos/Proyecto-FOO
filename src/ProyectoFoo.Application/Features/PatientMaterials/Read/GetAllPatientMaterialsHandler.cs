using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Shared.Models.PatientMaterial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.PatientMaterials.Read
{
    public class GetAllPatientMaterialsHandler(IPatientMaterialRepository patientMaterialRepository, IPatientRepository patientRepository) : IRequestHandler<GetAllPatientMaterialsQuery, List<PatientMaterialDto>>
    {
        private readonly IPatientMaterialRepository _patientMaterialRepository = patientMaterialRepository ?? throw new ArgumentNullException(nameof(patientMaterialRepository));
        private readonly IPatientRepository _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));

        public async Task<List<PatientMaterialDto>> Handle(GetAllPatientMaterialsQuery request, CancellationToken cancellationToken)
        {
            var patientExists = await _patientRepository.GetByIdAsync(request.PatientId);
            if (patientExists == null)
            {
                return [];
            }

            var materials = await _patientMaterialRepository.GetByPatientIdAsync(request.PatientId);
            return [.. materials.Select(m => new PatientMaterialDto
            {
                Id = m.Id,
                Title = m.Title,
                CreationDate = m.Date,
                Content = m.Content ?? string.Empty
            })];
        }
    }
}
