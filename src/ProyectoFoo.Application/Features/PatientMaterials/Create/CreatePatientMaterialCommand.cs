using MediatR;


namespace ProyectoFoo.Application.Features.PatientMaterials.Create
{
    public class CreatePatientMaterialCommand : IRequest<CreatePatientMaterialResponse>
    {
        public int PatientId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
