using Microsoft.Extensions.Logging;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.Features.Users.CRUD;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Shared.Models.User;

namespace ProyectoFoo.Application.ServiceExtension
{
    /// <summary>
    /// Servicio que contiene la lógica de negocio relacionada a usuarios.
    /// </summary>
    public class UserService(IUserRepository usuarioRepository, IEmailService emailService, IVerificationCodeService verificationCodeService, ILogger<UserService> logger) : IUserService
    {
        private readonly IUserRepository _userRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(_userRepository));
        private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService)); // Servicio de correo email para verificar
        private readonly IVerificationCodeService _verificationCodeService = verificationCodeService ?? throw new ArgumentNullException(nameof(verificationCodeService));
        private readonly ILogger<UserService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Obtiene un usuario por su identificador.
        /// </summary>
        public async Task<Usuario> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserById(id);
        }

        /// <summary>
        /// Marca a un usuario como verificado.
        /// </summary>
        public async Task<bool> MarkUserAsVerifiedAsync(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user is null)
            {
                return false;
            }

            user.IsVerified = true;
                await _userRepository.UpdateUsuario(user);
                return true;
        }

        /// <summary>
        /// Actualiza los datos del usuario con los valores recibidos desde un DTO.
        /// </summary>
        public async Task<Usuario> UpdateUserAsync(int id, UpdateUserDto updateUser)
        {
            var existingUser = await _userRepository.GetUserById(id);

            if (existingUser is null)
            {
                return null; 
            }

            if (updateUser.Name != null)
            {
                existingUser.Name = updateUser.Name.CapitalizeFirstLetter();
            }
            if (updateUser.Surname != null)
            {
                existingUser.Surname = updateUser.Surname.CapitalizeFirstLetter();
            }
            if (updateUser.Email != null)
            {
                existingUser.Email = updateUser.Email;
            }
            if (updateUser.Phone != null)
            {
                existingUser.Phone = updateUser.Phone;
            }
            if (updateUser.Title != null)
            {
                existingUser.Title = updateUser.Title.CapitalizeFirstLetter();
            }

            existingUser.LastAccesDate = DateTime.UtcNow;

            await _userRepository.UpdateUsuario(existingUser);

            return existingUser;
        }

        /// <summary>
        /// Actualiza la contraseña del usuario validando la contraseña actual.
        /// </summary>
        public async Task<bool> UpdateUserPasswordAsync(int id, UpdatePasswordDto updatePassword)
        {
            var existingUser = await _userRepository.GetUserById(id);

            if (existingUser is null)
            {
                return false; // El usuario no existe
            }

            if (!existingUser.VerifyPassword(updatePassword.CurrentPassword))
            {
                return false; // La contraseña actual es incorrecta
            }

            string newPasswordHash = Usuario.HashPassword(updatePassword.NewPassword);
           
            existingUser.SetPasswordHash(newPasswordHash);

            await _userRepository.UpdateUsuario(existingUser);

            return true;
        }

        /// <summary>
        /// Solicita un cambio de correo electrónico enviando un código de verificación al nuevo correo.
        /// </summary>
        public async Task<bool> RequestEmailChangeAsync(int userId, string newEmail)
        {
            var existingUser = await _userRepository.GetUserById(userId);

            if (existingUser is null)
            {
                return false;
            }

            string verificationCode = _verificationCodeService.GenerateCode(userId, "email-change");

            string subject = "Verificación de cambio de correo electrónico";
            string body = $"Tu código de verificación para cambiar tu correo electrónico es: {verificationCode}.Este código expirará en 15 minutos.";

            try
            {
                await _emailService.SendEmailAsync(newEmail, subject, body);
                return true;
            }
            catch (Exception)
            {
                _verificationCodeService.RemoveCode(userId, "email-change");
                return false;
            }
        }

        /// <summary>
        /// Confirma el cambio de correo electrónico si el código de verificación es válido.
        /// </summary>
        public async Task<bool> ConfirmEmailChangeAsync(int userId, string newEmail, string verificationCode)
        {
            if (!_verificationCodeService.ValidateCode(userId, "email-change", verificationCode))
            {
                return false; // Código inválido o expirado
            }

            var existingUser = await _userRepository.GetUserById(userId);
            if (existingUser is null)
            {
                return false; // Usuario no encontrado
            }

            existingUser.Email = newEmail;
            await _userRepository.UpdateUsuario(existingUser);

            _verificationCodeService.RemoveCode(userId, "email-change");

            return true;
        }
    }
}
