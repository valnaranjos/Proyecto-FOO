using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Shared;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IUserService
    {
        Task<Usuario> GetUserByIdAsync(int id);
        Task<Usuario> UpdateUserAsync(int id, UpdateUserDto updateUser);

        Task<bool> UpdateUserPasswordAsync(int id, UpdatePasswordDto updatePassword);
    }
}
