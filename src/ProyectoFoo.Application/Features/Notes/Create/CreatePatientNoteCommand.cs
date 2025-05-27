using MediatR;

namespace ProyectoFoo.Application.Features.Notes.Create
{
    public class CreatePatientNoteCommand : IRequest<CreatePatientNoteResponse>
    {
        public int PatientId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
