using ProyectoFoo.Shared.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.API.Models.Authentication
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) para el registro de un usuario.
    /// </summary>
    public class RegisterRequestDto
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


        [Required(ErrorMessage = "El Email es requerido.")]
        [EmailAddress(ErrorMessage = "El formato del Email no es válido.")]
        [StringLength(100, ErrorMessage = "El Email no puede exceder los 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Contraseña es requerida.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La Contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&+\-])[A-Za-z\d@$!%*?&+\-]{8,}$",
         ErrorMessage = "La Contraseña debe contener al menos una minúscula, una mayúscula, un número y un carácter especial.")]
        public string Password { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Número de teléfono no válido.")]
        [StringLength(20, ErrorMessage = "El número de móvil debe ser positivo.")]
        public string? Phone { get; set; }

        //Campo para validar la autenticación de dos pasos.
        public bool IsVerified { get; set; } = false;
    }
}
