using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Users
{
    /// <summary>
    /// DTO para recibir la información necesaria para actualizar la contraseña de un usuario.
    /// </summary>
    public class UpdatePasswordDto
    {
        /// <summary>
        /// La contraseña actual del usuario, necesaria para verificar antes de permitir el cambio.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public required string CurrentPassword { get; set; }


        /// <summary>
        /// La nueva contraseña que el usuario desea establecer.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }

        /// <summary>
        /// La confirmación de la nueva contraseña, debe coincidir con <see cref="NewPassword"/>.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "La contraseña debe contener al menos una letra minúscula, una letra mayúscula, un número y un carácter especial.")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
        public required string ConfirmNewPassword { get; set; }
    }
}
