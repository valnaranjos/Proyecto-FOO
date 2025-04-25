namespace ProyectoFoo.Shared.Models
{
    /// <summary>
    /// Modelo para las notas.
    /// </summary>
    public class Notas
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool Estado { get; set; } = true;
    }
}
