using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Users
{
    /// <summary>
    /// DTO para recibir la nueva dirección de correo electrónico cuando un usuario solicita cambiarla.
    /// </summary>
    public class ChangeEmailRequestDto
    {
        /// <summary>
        /// La nueva dirección de correo electrónico que el usuario desea utilizar.
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public required string NewEmail { get; set; }
    }
}
