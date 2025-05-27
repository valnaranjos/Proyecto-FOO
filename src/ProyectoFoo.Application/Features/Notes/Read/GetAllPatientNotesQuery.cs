using MediatR;
using ProyectoFoo.Shared.Models;
using System.Collections.Generic;

namespace ProyectoFoo.Application.Features.Notes.Read
{
    public class GetAllPatientNotesQuery : IRequest<List<PatientNoteDto>>
    {
        public int PatientId { get; set; }
    }
}
