using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProyectoFoo.Application.Contracts.Persistence;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.ServiceExtension
{
    public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<EmailService> _logger = logger;

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                string? host = smtpSettings["Host"];
                string? portStr = smtpSettings["Port"];
                string? senderEmail = smtpSettings["SenderEmail"];
                string? senderPassword = smtpSettings["SenderPassword"];
                
                if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(portStr) ||
                string.IsNullOrWhiteSpace(senderEmail) ||
                string.IsNullOrWhiteSpace(senderPassword))
                {
                    throw new InvalidOperationException("Faltan configuraciones SMTP requeridas.");
                }

                int port = int.Parse(portStr);
                using var smtpClient = new SmtpClient(host, port);
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);

                var mailMessage = new MailMessage(senderEmail, recipientEmail, subject, body);
                await smtpClient.SendMailAsync(mailMessage);

                _logger.LogInformation("Correo electrónico enviado exitosamente a {recipientEmail}", recipientEmail);
                _logger.LogInformation("Código de verificación enviado al correo {Email}: {Code}", recipientEmail, body);
            }
            catch (Exception)
            {
                _logger.LogError("Error al enviar el correo electrónico a {recipientEmail}", recipientEmail);
                throw; // Re-lanza la excepción para que el UserService pueda manejarla
            }
        }
    }
}

