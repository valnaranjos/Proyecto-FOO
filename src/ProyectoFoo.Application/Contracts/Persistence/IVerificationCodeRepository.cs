using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    /// <summary>
    /// Repositorio para la gestión de códigos de verificación.
    /// </summary>
    public interface IVerificationCodeRepository
    {
        Task AddVerificationCode(int userId, string code, string purpose, DateTime expiry);
        Task<(string? Code, DateTime? Expiry)> GetVerificationCode(int userId, string code, string purpose);
        Task RemoveVerificationCode(int userId, string code, string purpose);
        Task RemoveVerificationCodes(int userId, string purpose); 
    }
}
