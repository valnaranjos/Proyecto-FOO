using MediatR;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes
{
    public class CreateNoteCommand : IRequest<NoteResponseDto>
    {
        public CreateNoteDto Note { get; set; } = null!;
    }
}
