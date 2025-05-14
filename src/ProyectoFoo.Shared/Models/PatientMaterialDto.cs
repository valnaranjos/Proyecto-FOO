using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.Shared.Models
{
    public class PatientMaterialDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        public string? Content { get; set; }
    }
}
