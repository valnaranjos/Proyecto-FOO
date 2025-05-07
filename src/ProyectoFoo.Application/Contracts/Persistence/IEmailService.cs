using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    /// <summary>
    /// Define el contrato para un servicio de envío de correos electrónicos.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía un correo electrónico de forma asíncrona.
        /// </summary>
        /// <param name="recipientEmail">La dirección de correo electrónico del destinatario.</param>
        /// <param name="subject">El asunto del correo electrónico.</param>
        /// <param name="body">El cuerpo del correo electrónico.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task SendEmailAsync(string recipientEmail, string subject, string body);
    }
}
