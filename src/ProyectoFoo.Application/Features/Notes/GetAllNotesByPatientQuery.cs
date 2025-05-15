using MediatR;
using ProyectoFoo.Shared.Models;
using System.Collections.Generic;

namespace ProyectoFoo.Application.Features.Notes
{
    public class GetAllNotesByPatientQuery : IRequest<List<PatientNoteDto>>
    {
        public int PatientId { get; set; }

        public GetAllNotesByPatientQuery(int patientId)
        {
            PatientId = patientId;
        }

    }
}
