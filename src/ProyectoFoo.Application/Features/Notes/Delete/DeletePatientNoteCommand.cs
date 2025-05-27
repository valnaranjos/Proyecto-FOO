using MediatR;

namespace ProyectoFoo.Application.Features.Notes.Delete
{
    public class DeletePatientNoteCommand : IRequest<DeletePatientNoteResponse> 
    {
        public int PatientId { get; set; }
        public int NoteId { get; set; }
    }
}
