using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.API.Models.Authentication
{
    /// <summary>
    /// Objeto de transferencia de datos (DTO) para el registro de un usuario.
    /// </summary>
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "La Identificación es requerida.")]
       
        [Range(10, int.MaxValue, ErrorMessage = "La Identificación debe ser un número entero positivo mayor o igual a 10.")]
        public int Identification { get; set; }

        [Required(ErrorMessage = "El Nombre es requerido.")]
        [StringLength(50, ErrorMessage = "El Nombre no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El Nombre solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Apellido es requerido.")]
        [StringLength(50, ErrorMessage = "El Apellido no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El Apellido solo permite letras, acentos, la 'ñ' y espacios.")]
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

        //Campo para validar la autenticación de dos pasos.
        public bool IsVerified { get; set; } = false;
    }
}
