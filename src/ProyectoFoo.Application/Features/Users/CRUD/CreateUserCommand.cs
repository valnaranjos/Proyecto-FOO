using MediatR;
using ProyectoFoo.Shared.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFoo.Application.Features.Users.CRUD
{
    public class CreateUserCommand : IRequest<CreateUserResponse>
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El appelido es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        public string Email { get; set; } = string.Empty;

        
        public string? Phone { get; set; }
        
        public string? Title { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "La contraseña debe contener al menos una letra minúscula, una letra mayúscula, un número y un carácter especial.")]
        public string Password { get; set; } = string.Empty;
    }
}
