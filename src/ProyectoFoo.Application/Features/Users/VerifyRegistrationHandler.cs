using MediatR;
using Microsoft.Extensions.Logging;
using ProyectoFoo.Application.Contracts.Persistence;
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
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<VerifyRegistrationHandler> _logger;

        public VerifyRegistrationHandler(IUserRepository userRepository, IVerificationCodeRepository verificationCodeRepository, ITokenService tokenService, ILogger<VerifyRegistrationHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _verificationCodeRepository = verificationCodeRepository ?? throw new ArgumentNullException(nameof(verificationCodeRepository));
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
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return new VerifyRegistrationResponse { Success = false, Message = "Correo electrónico no encontrado." };
            }

            var (storedCode, expiry) = await _verificationCodeRepository.GetVerificationCode(user.Id, request.VerificationCode, "registration");

            if (storedCode == null)
            {
                return new VerifyRegistrationResponse { Success = false, Message = "Código de verificación inválido o expirado." };
            }

            user.IsVerified = true;
            await _userRepository.UpdateUsuario(user);
            await _verificationCodeRepository.RemoveVerificationCode(user.Id, request.VerificationCode, "registration");

            var token = _tokenService.GenerateToken(user);
            return new VerifyRegistrationResponse { Success = true, Message = "Cuenta verificada exitosamente.", Token = token };
        }
    }
}