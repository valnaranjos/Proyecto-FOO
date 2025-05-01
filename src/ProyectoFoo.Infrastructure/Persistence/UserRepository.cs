using Microsoft.EntityFrameworkCore;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Infrastructure.Context;
using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Infrastructure.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUserRepository
    {
        public UsuarioRepository(ApplicationContextSqlServer dbContext) : base(dbContext)
        {
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _dbContext.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsAsync(int identification)
        {
            return await _dbContext.Usuarios.AnyAsync(u => u.Identification == identification);
        }
    }
}