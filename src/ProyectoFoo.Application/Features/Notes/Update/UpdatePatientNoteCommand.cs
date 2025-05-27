using MediatR;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes.Update
{
    public class UpdatePatientNoteCommand : IRequest<UpdatePatientNoteResponse>
    {
        
        public int NoteId { get; set; }
        public int PatientId { get; set; }
        public PatientNoteDto Note { get; set; } = null!;
        
    }
}
