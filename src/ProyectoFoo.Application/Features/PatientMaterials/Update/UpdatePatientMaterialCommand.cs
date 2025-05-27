using MediatR;
using ProyectoFoo.Shared.Models;
using ProyectoFoo.Shared.Models.PatientMaterial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.PatientMaterials.Update
{
    public class UpdatePatientMaterialCommand : IRequest<UpdatePatientMaterialResponse>
    {
        public int PatientId { get; set; }
        public int MaterialId { get; set; }
        public required UpdatePatientMaterialDto Material { get; set; }
    }
}
