using MediatR;
using ProyectoFoo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProyectoFoo.Application.Features.PatientMaterials
{
    public class CreatePatientMaterialCommand : IRequest<CreatePatientMaterialResponse>
    {
        public int PatientId { get; set; }
        public CreatePatientMaterialDto Material { get; set; }
    }
}
