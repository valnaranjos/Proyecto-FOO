using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Domain.Entities
{
    public class PatientMaterial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Paciente")] // Clave foránea que relaciona con la tabla de Pacientes
        public int PatientId { get; set; }
        public Paciente Patient { get; set; }

        [Required(ErrorMessage = "El título del material es obligatorio.")]
        [StringLength(200, ErrorMessage = "El título del material no puede exceder los 200 caracteres.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "La fecha de la sesión es obligatoria.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "El contenido del material es obligatorio.")]
        public string Content { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ActualizationDate { get; set; }
    }
}
