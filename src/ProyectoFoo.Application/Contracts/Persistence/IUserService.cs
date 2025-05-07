using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Shared;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IUserService
    {
        Task<Usuario> GetUserByIdAsync(int id);
        Task<Usuario> UpdateUserAsync(int id, UpdateUserDto updateUser);

        //Task<IActionResult> UpdateUserPasswordAsync(int userId, UpdatePasswordDto updatePassword);
    }
}
