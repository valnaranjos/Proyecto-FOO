using MediatR;
using Microsoft.Extensions.Logging;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.ServiceExtension;
using ProyectoFoo.Domain.Entities;
using System.Xml.Linq;

namespace ProyectoFoo.Application.Features.Users.CRUD
{
    /// <summary>
    /// Manejador para el comando de creación de usuario.
    /// </summary>
    /// <remarks>
    /// Constructor del manejador de creación de usuario.
    /// </remarks>
    /// <param name="usuarioRepository">Repositorio para la entidad Usuario.</param>
    /// <param name="emailService">Servicio para el envío de correos electrónicos.</param>
    /// <param name="verificationCodeRepository">Repositorio para la entidad VerificationCode.</param>
    ///  <param name="logger">Servicio de logging.</param>
    public class CreateUserHandler(IUserRepository usuarioRepository, IEmailService emailService,
       IVerificationCodeService verificationCodeService, ILogger<CreateUserHandler> logger) : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly IUserRepository _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        private readonly IVerificationCodeService _verificationCodeService = verificationCodeService ?? throw new ArgumentNullException(nameof(verificationCodeService));
        private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        private readonly ILogger<CreateUserHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Maneja el comando de creación de usuario.
        /// </summary>
        /// <param name="request">El comando de creación de usuario.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Una respuesta que indica el resultado de la operación.</returns>
        public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Verificar si ya existe un usuario con el mismo correo electrónico
            var existingUserWithEmail = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (existingUserWithEmail != null)
            {
                if (!existingUserWithEmail.IsVerified)
                {
                    // El usuario existe pero no está verificado, generar un nuevo código
                    string newVerificationCode = _verificationCodeService.GenerateCode(existingUserWithEmail.Id, "registration");

                    string subject = "Reenvío de verificación de registro";
                    string body = $"Tu código de verificación es: {newVerificationCode}. Este código expirará en 15 minutos.";

                    try
                    {
                        await _emailService.SendEmailAsync(existingUserWithEmail.Email, subject, body);
                        _logger.LogInformation("Correo de reenvío de verificación enviado a {email}", existingUserWithEmail.Email);
                        return new CreateUserResponse { Success = true, Message = "Se ha reenviado el correo de verificación. Por favor, revisa tu bandeja de entrada." };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al reenviar el correo de verificación al usuario con ID {existingUserWithEmail.Id} y correo {existingUserWithEmail.Email}.", existingUserWithEmail.Id, existingUserWithEmail.Email);
                        return new CreateUserResponse { Success = false, Message = "Ocurrió un error al reenviar el correo de verificación. Por favor, inténtalo de nuevo más tarde." };
                    }
                }
                else
                {
                    // El usuario ya está verificado
                    return new CreateUserResponse
                    {
                        Success = false,
                        Message = $"Ya existe una cuenta verificada con el correo electrónico {request.Email}."
                    };
                }
            }

           

            // Si no existe un usuario con ese correo proceder con la creación normal
            var newUser = new Usuario
            {
                Name = request.Name.CapitalizeFirstLetter(),
                Surname = request.Surname.CapitalizeFirstLetter(),
                Email = request.Email,
                PasswordHash = Usuario.HashPassword(request.Password),
                Phone = request.Phone,
                CreatedDate = DateTime.UtcNow,
                LastAccesDate = DateTime.UtcNow,
                IsVerified = false
            };

            var createdUser = await _usuarioRepository.AddAsync(newUser);

            if (createdUser == null)
            {
                return new CreateUserResponse { Success = false, Message = "Error al registrar el usuario." };
            }

            string verificationCode = _verificationCodeService.GenerateCode(createdUser.Id, "registration");

            try
            {
                string subject = "Verificación de registro";
                string body = $"Tu código de verificación es: {verificationCode}. Este código expirará en 15 minutos.";
                await _emailService.SendEmailAsync(createdUser.Email, subject, body);
                _logger.LogInformation("Correo de verificación enviado a {email}", createdUser.Email);
                return new CreateUserResponse { Success = true, Message = "Cuenta creada. Por favor, verifica tu correo electrónico." };
            }
            catch (Exception)
            {
                try
                {
                    await _usuarioRepository.DeleteAsync(createdUser);
                    return new CreateUserResponse { Success = false, Message = "Ocurrió un error al enviar el correo de verificación. Por favor, inténtalo de nuevo más tarde." };
                }
                catch (Exception revertEx)
                {
                    _logger.LogError(revertEx, "Error al revertir la creación del usuario con ID {createdUser.Id} después de fallar el envío del correo.", createdUser.Id);
                    return new CreateUserResponse { Success = false, Message = "Ocurrió un error durante el registro. Por favor, contacta al soporte." };
                }
            }
        }
    }
}
