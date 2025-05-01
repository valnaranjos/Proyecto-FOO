using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IUserRepository : IAsyncRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<bool> ExistsAsync(int identification);
    }
}
