namespace ProyectoFoo.Shared.Models
{
    public class CreateNoteResponse
    {
        public PatientNoteDto? PatientNote { get; set; }

        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
