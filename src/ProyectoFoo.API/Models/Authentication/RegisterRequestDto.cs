using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.API.Models.Authentication
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "La Identificación es requerida.")]
        [StringLength(20, ErrorMessage = "La Identificación no puede exceder los 20 caracteres.")]
        public string Identification { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Nombre es requerido.")]
        [StringLength(100, ErrorMessage = "El Nombre no puede exceder los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Apellido es requerido.")]
        [StringLength(100, ErrorMessage = "El Apellido no puede exceder los 100 caracteres.")]
        public string Surname { get; set; } = string.Empty;


        [Required(ErrorMessage = "El Email es requerido.")]
        [EmailAddress(ErrorMessage = "El formato del Email no es válido.")]
        [StringLength(255, ErrorMessage = "El Email no puede exceder los 255 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Contraseña es requerida.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La Contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
         ErrorMessage = "La Contraseña debe contener al menos una minúscula, una mayúscula, un número y un carácter especial.")]
        public string Password { get; set; } = string.Empty;
    }
}
