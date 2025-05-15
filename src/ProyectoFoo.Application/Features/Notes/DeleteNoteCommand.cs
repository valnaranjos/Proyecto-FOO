using MediatR;

namespace ProyectoFoo.Application.Features.Notes
{
    public class DeleteNoteCommand : IRequest<DeleteNoteResponse> 
    {
        public int Id { get; set; }

        public DeleteNoteCommand(int id)
        {
            Id = id;
        }
    }
}
