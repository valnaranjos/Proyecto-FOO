using ProyectoFoo.Shared.ValidationAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFoo.Shared.Models.User
{
    public class UpdateUserDto
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")] 
        public string? Name { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")] 
        public string? Surname { get; set; }

        [EmailAddress(ErrorMessage = "Correo no válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public string? Email { get; set; }

        [Column(TypeName = "varchar(20)")]
        [OptionalPhone(ErrorMessage = "Número de móvil no válido.")]
        [StringLength(20, ErrorMessage = "El número de móvil debe ser positivo.")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El título puede contener letras y espacios.")]
        public string? Title { get; set; }
    }
}
