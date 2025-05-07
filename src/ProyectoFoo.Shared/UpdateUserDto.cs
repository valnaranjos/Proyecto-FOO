using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.Shared
{
    public class UpdateUserDto
    {
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")] 
        public string? Name { get; set; }

        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")] 
        public string? Surname { get; set; }
    }
}
