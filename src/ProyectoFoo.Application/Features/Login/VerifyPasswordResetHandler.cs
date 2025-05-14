using MediatR;
using Microsoft.Extensions.Logging;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.ServiceExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Login
{
    /// <summary>
    /// Comando para verificar el código de restablecimiento de contraseña.
    /// </summary>
    public class VerifyPasswordResetHandler : IRequestHandler<VerifyPasswordResetCommand, VerifyPasswordResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly ILogger<VerifyPasswordResetHandler> _logger;

        public VerifyPasswordResetHandler(
            IUserRepository userRepository,
            IVerificationCodeService verificationCodeService,
            ILogger<VerifyPasswordResetHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _verificationCodeService = verificationCodeService ?? throw new ArgumentNullException(nameof(verificationCodeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<VerifyPasswordResponse> Handle(VerifyPasswordResetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar al usuario por correo electrónico
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning("No se encontró un usuario con el correo {Email}.", request.Email);
                    return new VerifyPasswordResponse
                    {
                        Success = false,
                        Message = "No se encontró un usuario con el correo proporcionado."
                    };
                }

                // Validar el código de verificación
                if (!_verificationCodeService.ValidateCode(user.Id, "password-reset", request.Code))
                {
                    return new VerifyPasswordResponse
                    {
                        Success = false,
                        Message = "El código de verificación es inválido o ha expirado."
                    };
                }

                // Actualizar la contraseña del usuario
                user.SetPasswordHash(user.HashPassword(request.NewPassword));
                await _userRepository.UpdateAsync(user);

                // Eliminar el código de verificación usado
                _verificationCodeService.RemoveCode(user.Id, "password-reset");

                _logger.LogInformation("La contraseña fue restablecida exitosamente para el usuario con correo {Email}.", request.Email);
                return new VerifyPasswordResponse
                {
                    Success = true,
                    Message = "La contraseña fue restablecida exitosamente."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al verificar el código para el correo {Email}.", request.Email);
                return new VerifyPasswordResponse
                {
                    Success = false,
                    Message = "Ocurrió un error al procesar la solicitud. Inténtalo de nuevo más tarde."
                };
            }
        }
    }
}