using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Users
{
    /// <summary>
    /// Respuesta a la solicitud de verificación de registro.
    /// </summary>
    public class VerifyRegistrationResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        /// <summary>
        /// Token JWT generado tras la verificación exitosa.
        /// </summary>
        public string? Token { get; set; }
    }
}
