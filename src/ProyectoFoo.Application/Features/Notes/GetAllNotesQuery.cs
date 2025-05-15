using MediatR;
using ProyectoFoo.Shared.Models;
using System.Collections.Generic;

namespace ProyectoFoo.Application.Features.Notes
{
    public class GetNotesByPatientQuery : IRequest<List<NoteResponseDto>>
    {
        public int PatientId { get; set; }

        public GetNotesByPatientQuery(int patientId)
        {
            PatientId = patientId;
        }
    }
}
