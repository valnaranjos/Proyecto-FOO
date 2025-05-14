using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Users
{
    /// <summary>
    /// Comando para la verificación del registro de un usuario.
    /// </summary>
    public class VerifyRegistrationCommand : IRequest<VerifyRegistrationResponse>
    {
        /// <summary>
        /// El correo electrónico del usuario que se va a verificar.
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        public required string Email { get; set; }

        /// <summary>
        /// El código de verificación proporcionado por el usuario.
        /// </summary>
        [Required(ErrorMessage = "El código de verificación es obligatorio.")]
        public string VerificationCode { get; set; } = string.Empty;
    }
}
