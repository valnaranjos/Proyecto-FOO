using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IUserRepository : IAsyncRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<bool> ExistsAsync(string identification);
        Task UpdateUsuario(Usuario usuario);
        Task<Usuario?> GetUserById(int id);
        Task<Usuario?> GetByIdentificationAsync(string identification);


    }
}
