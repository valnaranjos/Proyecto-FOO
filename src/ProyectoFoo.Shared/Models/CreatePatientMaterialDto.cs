using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.Shared.Models
{
    public class CreatePatientMaterialDto
    {
        [Required(ErrorMessage = "El título del material es obligatorio.")]
        [StringLength(200, ErrorMessage = "El título del material no puede exceder los 200 caracteres.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "La fecha de la sesión es obligatoria.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "El contenido del material es obligatorio.")]
        public string? Content { get; set; }
    }
}
