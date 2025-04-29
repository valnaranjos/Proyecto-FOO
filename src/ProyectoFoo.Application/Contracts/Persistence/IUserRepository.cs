using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IUserRepository
    {
        Task<Usuario?> GetUsuarioPorEmail(string email);
        Task UpdateUsuario(Usuario usuario);
        Task AddUsuario(Usuario usuario);
    }
}
