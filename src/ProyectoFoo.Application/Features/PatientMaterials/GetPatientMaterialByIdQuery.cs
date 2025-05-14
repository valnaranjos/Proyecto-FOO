using MediatR;
using ProyectoFoo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.PatientMaterials
{
    public class GetPatientMaterialByIdQuery : IRequest<PatientMaterialDto>
    {
        public int PatientId { get; set; }
        public int MaterialId { get; set; }
    }
}
