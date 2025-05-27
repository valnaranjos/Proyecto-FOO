using MediatR;
using ProyectoFoo.Shared.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFoo.Application.Features.Users.CRUD
{
    public class CreateUserCommand : IRequest<CreateUserResponse>
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [NotNullOrWhitespace(ErrorMessage = "El nombre es obligatorio y no puede contener solo espacios.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El appelido es obligatorio.")]
        [NotNullOrWhitespace(ErrorMessage = "El apellido es obligatorio y no puede contener solo espacios.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        public string Email { get; set; } = string.Empty;

        [Column(TypeName = "varchar(20)")]
        [OptionalPhone(ErrorMessage = "Número de móvil no válido.")]
        [StringLength(20, ErrorMessage = "El número de móvil debe ser positivo.")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El título puede contener letras y espacios.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "La contraseña debe contener al menos una letra minúscula, una letra mayúscula, un número y un carácter especial.")]
        public string Password { get; set; } = string.Empty;
    }
}
