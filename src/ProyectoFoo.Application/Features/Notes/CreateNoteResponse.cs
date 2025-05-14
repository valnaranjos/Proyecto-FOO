namespace ProyectoFoo.Shared.Models
{
    public class CreateNoteResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int PatientId { get; set; }
    }
}
