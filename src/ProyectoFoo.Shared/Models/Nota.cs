namespace ProyectoFoo.Shared.Models
{
    /// <summary>
    /// Modelo Dto para las notas de paciente.
    /// </summary>
    public class Notes
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool Estado { get; set; } = true;
    }
}
