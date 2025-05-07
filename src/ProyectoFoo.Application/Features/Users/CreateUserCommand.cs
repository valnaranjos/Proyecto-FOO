using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.Application.Features.Users
{
    public class CreateUserCommand : IRequest<CreateUserResponse>
    {
        [Required(ErrorMessage = "El número de identificación es obligatorio.")]
        [Range(10, int.MaxValue, ErrorMessage = "El número de identificación debe ser positivo.")]
        public int Identification { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios.")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "La contraseña debe contener al menos una letra minúscula, una letra mayúscula, un número y un carácter especial.")]
        public string Password { get; set; } = string.Empty;
    }
}
