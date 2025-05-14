using MediatR;
using ProyectoFoo.Shared.Models;
using System.Collections.Generic;

namespace ProyectoFoo.Application.Features.Notes.Queries
{
    public class GetAllNotesQuery : IRequest<List<NoteResponseDto>>
    {
    }
}
