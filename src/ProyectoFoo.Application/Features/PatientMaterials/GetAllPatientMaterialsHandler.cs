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
    public class GetAllPatientMaterialsHandler : IRequestHandler<GetAllPatientMaterialsCommand, List<PatientMaterialDto>>
    {
        private readonly IPatientMaterialRepository _patientMaterialRepository;

        public GetAllPatientMaterialsHandler(IPatientMaterialRepository patientMaterialRepository)
        {
            _patientMaterialRepository = patientMaterialRepository ?? throw new ArgumentNullException(nameof(patientMaterialRepository));
        }

        public async Task<List<PatientMaterialDto>> Handle(GetAllPatientMaterialsCommand request, CancellationToken cancellationToken)
        {
            var materials = await _patientMaterialRepository.GetByPatientIdAsync(request.PatientId);
            return materials.Select(m => new PatientMaterialDto
            {
                Id = m.Id,
                Title = m.Title,
                Date = m.Date,
                Content = m.Content
            }).ToList();
        }

    }
}
