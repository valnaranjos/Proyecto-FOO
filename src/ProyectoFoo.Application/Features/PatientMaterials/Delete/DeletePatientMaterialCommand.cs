using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.PatientMaterials.Delete
{
    public class DeletePatientMaterialCommand : IRequest<DeletePatientMaterialResponse>
    {
        public int PatientId { get; set; }
        public int MaterialId { get; set; }
    }
}
