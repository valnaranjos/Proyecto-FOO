using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes
{
public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, bool>
{
    private readonly IPatientNoteRepository _noteRepository;

    public DeleteNoteCommandHandler(IPatientNoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public async Task<bool> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdAsync(request.Id, cancellationToken);

        if (note == null)
            return false;

        await _noteRepository.DeleteAsync(note, cancellationToken); 
        await _noteRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}}
