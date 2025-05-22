using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.API.Models.Authentication
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) para la solicitud de inicio de sesión.
    /// </summary>
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico no válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [MaxLength(100, ErrorMessage = "La contraseña no puede exceder los 100 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}
