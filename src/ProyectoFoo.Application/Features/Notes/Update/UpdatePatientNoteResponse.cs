using ProyectoFoo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes.Update
{
    public class UpdatePatientNoteResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public PatientNoteDto? PatientNote { get; set; }
    }
}
