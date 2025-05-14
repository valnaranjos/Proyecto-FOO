using MediatR;
using ProyectoFoo.Application.Common.Interfaces.Repositories;
using ProyectoFoo.Shared.Models;
using ProyectoFoo.Application.Features.Notes.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes.Handlers
{
    public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, NoteResponseDto?>
    {
        private readonly IPatientNoteRepository _noteRepository;

        public GetNoteByIdQueryHandler(IPatientNoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<NoteResponseDto?> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.GetByIdAsync(request.Id, cancellationToken);

            if (note == null)
                return null;

            return new NoteResponseDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedDate = note.CreatedDate,
                PacienteId = note.PatientId
            };
        }
    }
}