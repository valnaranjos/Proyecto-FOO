using MediatR;
using Microsoft.Extensions.Logging;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.ServiceExtension;
using ProyectoFoo.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Users
{

    public class VerifyRegistrationHandler : IRequestHandler<VerifyRegistrationCommand, VerifyRegistrationResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<VerifyRegistrationHandler> _logger;

        public VerifyRegistrationHandler(IUserRepository userRepository, IVerificationCodeService verificationCodeService, ITokenService tokenService, ILogger<VerifyRegistrationHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _verificationCodeService = verificationCodeService ?? throw new ArgumentNullException(nameof(verificationCodeService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Maneja el comando de verificación de registro.
        /// </summary>
        /// <param name="request">El comando de verificación de registro.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        public async Task<VerifyRegistrationResponse> Handle(VerifyRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar al usuario por correo electrónico
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning("No se encontró un usuario con el correo {Email}.", request.Email);
                    return new VerifyRegistrationResponse
                    {
                        Success = false,
                        Message = "No se encontró un usuario con el correo proporcionado."
                    };
                }

                // Validar el código de verificación
                if (!_verificationCodeService.ValidateCode(user.Id, "registration", request.VerificationCode))
                {
                    _logger.LogWarning("Código de verificación inválido o expirado para el usuario con correo {Email}.", request.Email);
                    return new VerifyRegistrationResponse
                    {
                        Success = false,
                        Message = "El código de verificación es inválido o ha expirado."
                    };
                }

                // Marcar al usuario como verificado
                user.IsVerified = true;
                await _userRepository.UpdateUsuario(user);

                // Eliminar el código de verificación usado
                _verificationCodeService.RemoveCode(user.Id, "registration");

                // Generar un token JWT
                var token = _tokenService.GenerateToken(user);

                _logger.LogInformation("Usuario con correo {Email} verificado exitosamente.", request.Email);
                return new VerifyRegistrationResponse
                {
                    Success = true,
                    Message = "Usuario verificado exitosamente.",
                    Token = token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al verificar el registro para el correo {Email}.", request.Email);
                return new VerifyRegistrationResponse
                {
                    Success = false,
                    Message = "Ocurrió un error al procesar la solicitud. Inténtalo de nuevo más tarde."
                };
            }
        }
    }
}