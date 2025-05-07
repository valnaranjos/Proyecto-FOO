using Microsoft.Extensions.Logging;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Shared;
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
        private readonly ILogger<UserService> _logger;

        //Almacenamiento temporal en memoria para códigos de verificación, 15 min de espera
        private static ConcurrentDictionary<(int, string), (string Code, DateTime Expiry)> _emailVerificationCodes = new();
        private const int VerificationCodeExpiryMinutes = 15;

        public UserService(IUserRepository usuarioRepository, IEmailService emailService, ILogger<UserService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
            _logger = logger;
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

            // Generar código de verificación único
            string verificationCode = Guid.NewGuid().ToString()[..8].ToUpper();

            // Almacenar temporalmente el código
            var expiryTime = DateTime.UtcNow.AddMinutes(VerificationCodeExpiryMinutes);
            _emailVerificationCodes.AddOrUpdate((userId, newEmail), (verificationCode, expiryTime), (key, oldValue) => (verificationCode, expiryTime));

            // Enviar correo electrónico con el código
            string subject = "Verificación de cambio de correo electrónico";
            string body = $"Tu código de verificación para cambiar tu correo electrónico es: {verificationCode}. Este código expirará en {VerificationCodeExpiryMinutes} minutos.";

            try
            {
                await _emailService.SendEmailAsync(newEmail, subject, body);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al enviar el correo de verificación a {newEmail}: {ex}", newEmail, ex);
                // Limpiar el código temporal en caso de fallo de envío
                _emailVerificationCodes.TryRemove((userId, newEmail), out var _);
                return false;
            }
        }

        public async Task<bool> ConfirmEmailChangeAsync(int userId, string newEmail, string verificationCode)
        {
            if (_emailVerificationCodes.TryGetValue((userId, newEmail), out var storedCodeInfo))
            {
                if (storedCodeInfo.Code == verificationCode && storedCodeInfo.Expiry > DateTime.UtcNow)
                {
                    var existingUser = await _usuarioRepository.GetUserById(userId);
                    if (existingUser != null)
                    {
                        existingUser.Email = newEmail;
                        await _usuarioRepository.UpdateUsuario(existingUser);
                        _emailVerificationCodes.TryRemove((userId, newEmail), out var _); // Limpiar el código
                        return true;
                    }
                    return false; // Usuario no encontrado
                }
                else
                {
                    return false; // Código inválido o expirado
                }
            }
            else
            {
                return false; // No se encontró solicitud de cambio para este usuario y correo
            }
        }
    }
}
