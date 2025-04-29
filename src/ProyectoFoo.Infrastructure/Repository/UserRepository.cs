using Microsoft.EntityFrameworkCore;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Shared.Models;
using ProyectoFoo.Infrastructure.Context;
using System.Threading.Tasks;
using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Infrastructure.Repositories
{
    public class UsuarioRepository : IUserRepository
    {
        private readonly ApplicationContextSqlServer _context;

        public UsuarioRepository(ApplicationContextSqlServer context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetUsuarioPorEmail(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateUsuario(Usuario usuario)
        {
            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task AddUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }
    }
}