using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.Shared.Models
{
    public class CreateNoteDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "El título no puede exceder los 50 caracteres.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public int PatientId { get; set; }
    }
}
