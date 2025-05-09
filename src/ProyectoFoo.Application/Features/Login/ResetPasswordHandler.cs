using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.Features.Login;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using ProyectoFoo.Application.ServiceExtension;
using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Application.Features.Login
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly ILogger<ResetPasswordHandler> _logger;
        private readonly IEmailService _emailService;


        public ResetPasswordHandler(IUserRepository userRepository,
           IVerificationCodeService verificationCodeService,
           IEmailService emailService,
           ILogger<ResetPasswordHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _verificationCodeService = verificationCodeService ?? throw new ArgumentNullException(nameof(verificationCodeService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar al usuario por correo electrónico
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning("No se encontró un usuario con el correo {Email}.", request.Email);
                    return new ResetPasswordResponse
                    {
                        Success = false,
                        Message = "No se encontró un usuario con el correo proporcionado."
                    };
                }

                // Generar el código de verificación y enviarlo (podrías integrar con un servicio de email o SMS)
                var code = _verificationCodeService.GenerateCode(user.Id, "password-reset");

                // Aquí envia el código por correo electrónico
                string subject = "Reestablece tu contraseña";
                string body = $"Tu código de verificación es: {code}. Este código expirará en 15 minutos.";
                await _emailService.SendEmailAsync(user.Email, subject, body);

                _logger.LogInformation("Código de verificación enviado al usuario con correo {Email}.", request.Email);

                return new ResetPasswordResponse
                {
                    Success = true,
                    Message = "Te hemos enviado un código de verificación."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al generar el código de verificación para el correo {Email}.", request.Email);
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Ocurrió un error al procesar la solicitud. Inténtalo de nuevo más tarde."
                };
            }
        }
    }    
}
