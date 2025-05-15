using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes
{
public class DeleteNoteHandler : IRequestHandler<DeleteNoteCommand, DeleteNoteResponse>
{
    private readonly IPatientNoteRepository _noteRepository;

    public DeleteNoteHandler(IPatientNoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public async Task<DeleteNoteResponse> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _noteRepository.GetByIdAsync(request.Id);

        if (note is null)
            {
                return new DeleteNoteResponse
                {
                    Success = false,
                    Message = "Note not found"
                };
            }

            try
            {
                _noteRepository.DeleteAsync(note);

                return new DeleteNoteResponse
                {
                    Success = true,
                    Message = "Nota eliminada correctamente."
                };
            }
            catch (Exception ex)
            {
                return new DeleteNoteResponse
                {
                    Success = false,
                    Message = $"Error al eliminar la nota: {ex.Message}"
                };
            }
        }
}}
