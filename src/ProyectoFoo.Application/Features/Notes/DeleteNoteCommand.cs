using MediatR;

namespace ProyectoFoo.Application.Features.Notes
{
    public class DeleteNoteCommand : IRequest<bool> 
    {
        public int Id { get; set; }

        public DeleteNoteCommand(int id)
        {
            Id = id;
        }
    }
}
