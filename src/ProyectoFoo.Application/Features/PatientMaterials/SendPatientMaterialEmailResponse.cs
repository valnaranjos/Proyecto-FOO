using ProyectoFoo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.PatientMaterials
{
    public class SendPatientMaterialEmailResponse
    {
        /// <summary>
        /// Indica si el intento de envío del correo fue exitoso.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo sobre el resultado de la operación (éxito o error).
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Dto del material del paciente que fue enviado en el email.
        /// </summary>
        
        public PatientMaterialDto? PatientMaterial { get; set; }
    }
}
