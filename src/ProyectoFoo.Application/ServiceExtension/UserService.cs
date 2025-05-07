using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.ServiceExtension
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _usuarioRepository;

        public UserService(IUserRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario> GetUserByIdAsync(int id)
        {
            return await _usuarioRepository.GetUserById(id);
        }

        public async Task<Usuario> UpdateUserAsync(int id, UpdateUserDto updateUser)
        {
            var existingUser = await _usuarioRepository.GetUserById(id);

            if (existingUser == null)
            {
                return null; // El usuario no existe
            }

            // Actualizar solo las propiedades permitidas desde el DTO
            if (updateUser.Name != null)
            {
                existingUser.Name = updateUser.Name;
            }
            if (updateUser.Surname != null)
            {
                existingUser.Surname = updateUser.Surname;
            }

            // Actualizar la fecha del último acceso (opcional)
            existingUser.LastAccesDate = DateTime.UtcNow;

            await _usuarioRepository.UpdateUsuario(existingUser); // Actualiza la BD

            return existingUser;

        }        
    }
}
