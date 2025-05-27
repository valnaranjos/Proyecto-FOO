using ProyectoFoo.Shared.Models.PatientMaterial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.PatientMaterials.Update
{
    public class UpdatePatientMaterialResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public PatientMaterialDto? PatientMaterial { get; set; }
    }
}
