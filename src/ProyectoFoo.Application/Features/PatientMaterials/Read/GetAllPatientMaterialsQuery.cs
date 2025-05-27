using MediatR;
using ProyectoFoo.Shared.Models.PatientMaterial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.PatientMaterials.Read
{
    public class GetAllPatientMaterialsQuery : IRequest<List<PatientMaterialDto>>
    {
        public int PatientId { get; set; }
    }
}
