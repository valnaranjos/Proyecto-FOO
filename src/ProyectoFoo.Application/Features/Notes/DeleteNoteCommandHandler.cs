using MediatR;
using ProyectoFoo.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes
{
    public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public DeleteNoteCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
        {
            var note = await _context.Notes.FindAsync(new object[] { request.Id }, cancellationToken);
            if (note == null)
                throw new Exception("Note not found");

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
