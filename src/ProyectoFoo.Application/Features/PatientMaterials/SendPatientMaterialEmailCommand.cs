using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.PatientMaterials
{
    public class SendPatientMaterialEmailCommand : IRequest<SendPatientMaterialEmailResponse>
    {
        /// <summary>
        /// ID del paciente al que se le va a enviar el material.
        /// </summary>
        public int PatientId { get; set; }

        /// <summary>
        /// ID del material que se va a enviar.
        /// </summary>
        public int MaterialId { get; set; }

        /// <summary>
        /// ID del usuario que está realizando la acción de envío (para personalizar la firma del email).
        /// </summary>
        public int UserId { get; set; }
    }
}
