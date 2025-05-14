using MediatR;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes
{
    public class UpdateNoteCommand : IRequest<NoteResponseDto>
    {
        public int Id { get; set; }
        public UpdateNoteDto Note { get; set; } = null!;
    }
}
