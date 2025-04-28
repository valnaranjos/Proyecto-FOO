namespace ProyectoFoo.API.Models
{
    public class UpdatePatientDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateOnly? Birthdate { get; set; }
        public int? Identification { get; set; } // Hacer nullable si no siempre se actualiza
        public string? Sex { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Modality { get; set; }
        public string? Diagnosis { get; set; }
        public string? Institution { get; set; }
        // No incluir el Id aquí, ya que se toma de la URL
    }
}
