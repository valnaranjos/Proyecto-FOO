using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProyectoFoo.Application.Contracts.Persistence;
using System.Net;
using System.Net.Mail;

namespace ProyectoFoo.Application.ServiceExtension
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        private readonly IConfiguration _configuration = configuration;

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
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"Error al enviar correo");
            }
        }
    }
}

