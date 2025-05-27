using ProyectoFoo.Shared.Models;

namespace ProyectoFoo.Application.Features.Notes.Create
{
    public class CreatePatientNoteResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int PatientNoteId { get; set; }
        public PatientNoteDto? Note { get; set; }
    }
}
