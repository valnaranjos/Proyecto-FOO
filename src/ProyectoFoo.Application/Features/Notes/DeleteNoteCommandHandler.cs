using MediatR;
using ProyectoFoo.Application.Common.Interfaces.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes
{
    public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, Unit>
    {
        private readonly IPatientNoteRepository _noteRepository;

        public DeleteNoteCommandHandler(IPatientNoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<Unit> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.GetByIdAsync(request.Id, cancellationToken);

            if (note == null)
                throw new KeyNotFoundException($"No se encontró la nota con ID {request.Id}");

            _noteRepository.Remove(note);
            await _noteRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}