using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IUserRepository : IAsyncRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task UpdateUsuario(Usuario usuario);
        Task<Usuario?> GetUserById(int id);

    }
}
