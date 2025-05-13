using Microsoft.Extensions.Logging;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.Features.Users;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.ServiceExtension
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _usuarioRepository;
        private readonly IEmailService _emailService; // Servicio de correo email para verificar
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository usuarioRepository, IEmailService emailService, IVerificationCodeService verificationCodeService, ILogger<UserService> logger)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _verificationCodeService = verificationCodeService ?? throw new ArgumentNullException(nameof(verificationCodeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Usuario> GetUserByIdAsync(int id)
        {
            return await _usuarioRepository.GetUserById(id);
        }

        public async Task<bool> MarkUserAsVerifiedAsync(int userId)
        {
            var user = await _usuarioRepository.GetUserById(userId);
            if (user != null)
            {
                user.IsVerified = true;
                await _usuarioRepository.UpdateUsuario(user);
                return true;
            }
            return false;
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

            // Actualizar la fecha del último acceso (opcional)
            existingUser.LastAccesDate = DateTime.UtcNow;

            await _usuarioRepository.UpdateUsuario(existingUser); // Actualiza la BD

            return existingUser;
        }

        public async Task<bool> UpdateUserPasswordAsync(int id, UpdatePasswordDto updatePassword)
        {
            // 1. Buscar el usuario en la base de datos por su ID
            var existingUser = await _usuarioRepository.GetUserById(id);

            if (existingUser == null)
            {
                return false; // El usuario no existe
            }

            // 2. Verificar la contraseña actual
            if (!existingUser.VerifyPassword(updatePassword.CurrentPassword))
            {
                return false; // La contraseña actual es incorrecta
            }

            string newPasswordHash = existingUser.HashPassword(updatePassword.NewPassword);
           
            existingUser.SetPasswordHash(newPasswordHash);

            await _usuarioRepository.UpdateUsuario(existingUser);

            return true;
        }

        public async Task<bool> RequestEmailChangeAsync(int userId, string newEmail)
        {
            var existingUser = await _usuarioRepository.GetUserById(userId);

            if (existingUser == null)
            {
                return false;
            }

            // Generar código de verificación
            string verificationCode = _verificationCodeService.GenerateCode(userId, "email-change");

            // Enviar correo electrónico con el código
            string subject = "Verificación de cambio de correo electrónico";
            string body = $"Tu código de verificación para cambiar tu correo electrónico es: {verificationCode}.Este código expirará en 15 minutos.";

            try
            {
                await _emailService.SendEmailAsync(newEmail, subject, body);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al enviar el correo de verificación a {newEmail}: {ex}", newEmail, ex);
                _verificationCodeService.RemoveCode(userId, "email-change");
                return false;
            }
        }

        public async Task<bool> ConfirmEmailChangeAsync(int userId, string newEmail, string verificationCode)
        {
            // Validar el código de verificación
            if (!_verificationCodeService.ValidateCode(userId, "email-change", verificationCode))
            {
                return false; // Código inválido o expirado
            }

            var existingUser = await _usuarioRepository.GetUserById(userId);
            if (existingUser == null)
            {
                return false; // Usuario no encontrado
            }

            // Actualizar el correo electrónico del usuario
            existingUser.Email = newEmail;
            await _usuarioRepository.UpdateUsuario(existingUser);

            // Eliminar el código de verificación usado
            _verificationCodeService.RemoveCode(userId, "email-change");

            return true;
        }
    }
}
