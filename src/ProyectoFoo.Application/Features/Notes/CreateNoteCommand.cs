using MediatR;
using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes
{
    public class CreateNoteCommand : IRequest<CreateNoteResponse>
    {
        public CreateNoteDto Note { get; set; } = null!;
        public int PatientId { get; set; }
    }
}
