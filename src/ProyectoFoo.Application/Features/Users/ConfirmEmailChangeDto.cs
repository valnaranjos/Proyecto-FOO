using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Users
{
    /// <summary>
    /// DTO para recibir el código de verificación cuando un usuario confirma el cambio de correo electrónico.
    /// </summary>
    public class ConfirmEmailChangeDto
    {
        /// <summary>
        /// El código de verificación que el usuario recibió en su nueva dirección de correo electrónico.
        /// </summary>
        [Required(ErrorMessage = "El código de verificación es requerido.")]
        public required string VerificationCode { get; set; }
    }
}
