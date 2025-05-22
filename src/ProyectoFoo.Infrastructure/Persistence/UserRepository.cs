using Microsoft.EntityFrameworkCore;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Infrastructure.Context;
using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Infrastructure.Persistence
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUserRepository
    {
        private new readonly ApplicationContextSqlServer _dbContext;
        public UsuarioRepository(ApplicationContextSqlServer dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _dbContext.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<Usuario?> GetUserById(int id)
        {
            return await _dbContext.Usuarios.FindAsync(id);
        }
        public async Task<bool> ExistsAsync(string identification)
        {
            return await _dbContext.Usuarios.AnyAsync(u => u.Identification == identification);
        }

        public async Task UpdateUsuario(Usuario usuario)
        {
            await UpdateAsync(usuario);
        }

        public async Task<Usuario?> GetByIdentificationAsync(string identification)
        {
            return await _dbContext.Usuarios.FirstOrDefaultAsync(u => u.Identification == identification);
        }
    }
}