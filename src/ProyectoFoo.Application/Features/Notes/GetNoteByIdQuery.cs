using MediatR;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes.Queries
{
    public class GetNoteByIdQuery : IRequest<PatientNoteDto?>
    {
        public int NoteId { get; set; }
        public GetNoteByIdQuery(int noteId)
        {
            NoteId = noteId;
        }
    }
}
