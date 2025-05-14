using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Infrastructure.Context;
using ProyectoFoo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Infrastructure.Persistence
{
    /// <summary>
    /// Implementación del repositorio para la gestión de códigos de verificación.
    /// </summary>
    public class VerificationCodeRepository : BaseRepository<VerificationCode>, IVerificationCodeRepository
    {
        private new readonly ApplicationContextSqlServer _dbContext;
        public VerificationCodeRepository(ApplicationContextSqlServer dbContext) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddVerificationCode(int userId, string code, string purpose, DateTime expiry)
        {
            var verificationCode = new VerificationCode
            {
                UserId = userId,
                Code = code,
                Purpose = purpose,
                Expiry = expiry
            };
            await _dbContext.VerificationCodes.AddAsync(verificationCode);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<(string? Code, DateTime? Expiry)> GetVerificationCode(int userId, string code, string purpose)
        {
            var verificationCode = await _dbContext.VerificationCodes
                .Where(vc => vc.UserId == userId && vc.Code == code && vc.Purpose == purpose && vc.Expiry > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            return (verificationCode?.Code, verificationCode?.Expiry);
        }

        public async Task RemoveVerificationCode(int userId, string code, string purpose)
        {
            var verificationCode = await _dbContext.VerificationCodes
                .Where(vc => vc.UserId == userId && vc.Code == code && vc.Purpose == purpose)
                .FirstOrDefaultAsync();

            if (verificationCode != null)
            {
                _dbContext.VerificationCodes.Remove(verificationCode);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveVerificationCodes(int userId, string purpose)
        {
            var codesToRemove = await _dbContext.VerificationCodes
                .Where(vc => vc.UserId == userId && vc.Purpose == purpose)
                .ToListAsync();

            if (codesToRemove.Count > 0)
            {
                _dbContext.VerificationCodes.RemoveRange(codesToRemove);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
