using Microsoft.Extensions.Logging;
using ProyectoFoo.Application.Contracts.Persistence;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.ServiceExtension
{
    public class VerificationCodeService : IVerificationCodeService
    {
        private static readonly ConcurrentDictionary<(int UserId, string Purpose), (string Code, DateTime Expiry)> _verificationCodes = new();
        private const int VerificationCodeExpiryMinutes = 15;
        private readonly ILogger<VerificationCodeService> _logger;

        public VerificationCodeService(ILogger<VerificationCodeService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// Genera y almacena un código de verificación para un propósito específico.
        /// </summary>
        public string GenerateCode(int userId, string purpose)
        {
            string code = Guid.NewGuid().ToString()[..8].ToUpper();
            DateTime expiry = DateTime.UtcNow.AddMinutes(VerificationCodeExpiryMinutes);

            _verificationCodes.AddOrUpdate((userId, purpose), (code, expiry), (key, oldValue) => (code, expiry));

            _logger.LogInformation("Código de verificación generado: {Code} para el propósito: {Purpose}", code, purpose);


            return code;
        }

        /// <summary>
        /// Valida un código de verificación para un propósito específico.
        /// </summary>
        public bool ValidateCode(int userId, string purpose, string code)
        {
            if (_verificationCodes.TryGetValue((userId, purpose), out var storedCodeInfo))
            {
                if (storedCodeInfo.Code == code && storedCodeInfo.Expiry > DateTime.UtcNow)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Elimina un código de verificación después de su uso.
        /// </summary>
        public void RemoveCode(int userId, string purpose)
        {
            // Eliminar códigos expirados automáticamente
            var now = DateTime.UtcNow;
            foreach (var key in _verificationCodes.Keys)
            {
                if (_verificationCodes[key].Expiry <= now)
                {
                    _verificationCodes.TryRemove(key, out _);
                }
            }

            // Eliminar el código específico
            _verificationCodes.TryRemove((userId, purpose), out _);
        }
    }
}
