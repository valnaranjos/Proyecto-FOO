using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.Domain.Entities
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El título no puede exceder los 50 caracteres.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int PatientId { get; set; }
        public Paciente Paciente { get; set; } = null!;
    }
}
