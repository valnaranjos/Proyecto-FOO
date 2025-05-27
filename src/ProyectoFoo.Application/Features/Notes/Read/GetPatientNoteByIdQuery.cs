using MediatR;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes.Read
{
    public class GetPatientNoteByIdQuery : IRequest<PatientNoteDto?>
    {
        public int PatientId { get; set; }
        public int NoteId { get; set; }
        
    }
}
