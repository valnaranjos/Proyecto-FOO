using MediatR;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.PatientMaterials.Read
{
    public class GetPatientMaterialByIdQuery : IRequest<PatientMaterialDto>
    {
        public int PatientId { get; set; }
        public int MaterialId { get; set; }
    }
}
