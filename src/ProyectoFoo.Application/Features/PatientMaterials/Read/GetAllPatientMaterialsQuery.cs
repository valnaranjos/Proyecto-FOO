using MediatR;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.PatientMaterials.Read
{
    public class GetAllPatientMaterialsQuery : IRequest<List<PatientMaterialDto>>
    {
        public int PatientId { get; set; }
    }
}
