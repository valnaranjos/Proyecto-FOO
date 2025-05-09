using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Login
{
    /// <summary>
    /// Respuesta a la solicitud de restablecimiento de contraseña.
    /// </summary>
    public class ResetPasswordResponse
    {
        // <summary>
        /// Indica si la operación fue exitosa.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo del resultado.
        /// </summary>
        public string? Message { get; set; }
    }
}
