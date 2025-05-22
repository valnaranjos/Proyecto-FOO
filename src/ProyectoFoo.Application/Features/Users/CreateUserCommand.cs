using MediatR;
using ProyectoFoo.Shared.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.Application.Features.Users
{
    public class CreateUserCommand : IRequest<CreateUserResponse>
    {
        [Required(ErrorMessage = "El número de identificación es obligatorio.")]
        [StringLength(20, ErrorMessage = "La identificación no puede exceder los 20 caracteres.")] 
        [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "La identificación solo puede contener letras, números y guiones.")] // Nuevo: Validar formato alfanumérico
        public string Identification { get; set; } = string.Empty;


        [NotNullOrWhitespace(ErrorMessage = "El nombre es obligatorio y no puede contener solo espacios.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Name { get; set; } = string.Empty;

        [NotNullOrWhitespace(ErrorMessage = "El apellido es obligatorio y no puede contener solo espacios.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Número de teléfono no válido.")]
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
