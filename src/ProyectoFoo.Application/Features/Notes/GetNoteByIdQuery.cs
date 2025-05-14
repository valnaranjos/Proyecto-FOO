using MediatR;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes.Queries
{
    public class GetNoteByIdQuery : IRequest<NoteResponseDto?>
    {
        public int Id { get; set; }

        public GetNoteByIdQuery(int id)
        {
            Id = id;
        }
    }
}
