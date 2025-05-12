using ProyectoFoo.Application.Features.Users;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Shared;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    /// <summary>
    /// Define las operaciones relacionadas con la gestión de usuarios.
    /// </summary>
    /// /// <summary>
    public interface IUserService
    {
        /// Obtiene un usuario por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del usuario a buscar.</param>
        /// <returns>Una tarea que representa la operación asíncrona y contiene el <see cref="Usuario"/> encontrado, o null si no existe.</returns>
        Task<Usuario> GetUserByIdAsync(int id);

        /// <summary>
        /// Actualiza la información de un usuario existente de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del usuario a actualizar.</param>
        /// <param name="updateUser">Un DTO que contiene la información actualizada del usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona y contiene el <see cref="Usuario"/> actualizado, o null si el usuario no existe.</returns>
        Task<Usuario> UpdateUserAsync(int id, UpdateUserDto updateUser);

        /// <summary>
        /// Actualiza la contraseña de un usuario de forma asíncrona.
        /// </summary>
        /// <param name="userId">El ID del usuario cuya contraseña se va a actualizar.</param>
        /// <param name="updatePassword">Un DTO que contiene la contraseña actual y la nueva contraseña.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve true si la contraseña se actualizó correctamente, false en caso contrario.</returns>
        Task<bool> UpdateUserPasswordAsync(int id, UpdatePasswordDto updatePassword);

        /// <summary>
        /// Inicia el proceso de cambio de correo electrónico para un usuario, generando y enviando un código de verificación.
        /// </summary>
        /// <param name="userId">El ID del usuario que solicita el cambio de correo.</param>
        /// <param name="newEmail">La nueva dirección de correo electrónico a la que se enviará el código.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve true si la solicitud se procesó correctamente, false en caso contrario.</returns>
        Task<bool> RequestEmailChangeAsync(int userId, string newEmail);


        /// <summary>
        /// Confirma el cambio de correo electrónico de un usuario verificando el código proporcionado.
        /// </summary>
        /// <param name="userId">El ID del usuario que confirma el cambio de correo.</param>
        /// <param name="newEmail">La nueva dirección de correo electrónico que se está confirmando.</param>
        /// <param name="verificationCode">El código de verificación proporcionado por el usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve true si el cambio de correo se confirmó correctamente, false en caso contrario.</returns>
        Task<bool> ConfirmEmailChangeAsync(int userId, string newEmail, string verificationCode);


        Task<bool> MarkUserAsVerifiedAsync(int userId);
    }
}
