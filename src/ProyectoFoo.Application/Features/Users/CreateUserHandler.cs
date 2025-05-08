using MediatR;
using Microsoft.Extensions.Logging;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Application.Features.Users
{
    /// <summary>
    /// Manejador para el comando de creación de usuario.
    /// </summary>
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly IUserRepository _usuarioRepository;
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<CreateUserHandler> _logger;


        /// <summary>
        /// Constructor del manejador de creación de usuario.
        /// </summary>
        /// <param name="usuarioRepository">Repositorio para la entidad Usuario.</param>
        /// <param name="emailService">Servicio para el envío de correos electrónicos.</param>
        /// <param name="verificationCodeRepository">Repositorio para la entidad VerificationCode.</param>
        ///  <param name="logger">Servicio de logging.</param>
        public CreateUserHandler(IUserRepository usuarioRepository, IEmailService emailService, IVerificationCodeRepository verificationCodeRepository, ILogger<CreateUserHandler> logger)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _verificationCodeRepository = verificationCodeRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
        }

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

                    // Generar nuevo código de verificación
                    string verificationCode = Guid.NewGuid().ToString()[..8].ToUpper();
                    var expiryTime = DateTime.UtcNow.AddMinutes(15);

                    //Importante: Eliminar códigos de verificación anteriores para este usuario y propósito ***
                    await _verificationCodeRepository.RemoveVerificationCodes(existingUserWithEmail.Id, "registration");
                    await _verificationCodeRepository.AddVerificationCode(existingUserWithEmail.Id, verificationCode, "registration", expiryTime);

                    // Enviar correo electrónico de verificación
                    string subject = "Reenvío de verificación de registro"; // Asunto más claro
                    string body = $"Se ha reenviado el código de verificación para tu cuenta en Proyecto Foo. Por favor, utiliza el siguiente código para verificar tu cuenta: {verificationCode}. Este código expirará en 15 minutos.";

                    try
                    {
                        await _emailService.SendEmailAsync(existingUserWithEmail.Email, subject, body);
                        _logger.LogInformation("Correo de reenvío de verificación enviado a {email}", existingUserWithEmail.Email);
                        return new CreateUserResponse { Success = true, Message = "Se ha reenviado el correo de verificación. Por favor, revisa tu bandeja de entrada." };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al reenviar el correo de verificación al usuario con ID {existingUserWithEmail.Id} y correo {existingUserWithEmail.Email}.", existingUserWithEmail.Id, existingUserWithEmail.Email);
                       
                        return new CreateUserResponse { Success = false, Message = "Ocurrió un error al reenviar el correo de verificación. Por favor, inténtalo de nuevo más tarde." }; //Mensaje mas especifico
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

            // Si no existe un usuario con ese correo, proceder con la creación normal
            var newUser = new Usuario(
                id: 0,
                nombre: request.Name,
                correo: request.Email,
                contrasena: request.Password
            )
            {
                Surname = request.Surname,
                Identification = request.Identification,
                IsVerified = false
            };

            // Agregar el nuevo usuario a la base de datos
            var createdUser = await _usuarioRepository.AddAsync(newUser);

            if (createdUser == null)
            {
                return new CreateUserResponse { Success = false, Message = "Error al registrar el usuario." };
            }

            // Generar código de verificación
            string verificationCodeNuevo = Guid.NewGuid().ToString()[..8].ToUpper();
            var expiryTimeNuevo = DateTime.UtcNow.AddMinutes(15);

            // Almacenar el código de verificación
            await _verificationCodeRepository.AddVerificationCode(createdUser.Id, verificationCodeNuevo, "registration", expiryTimeNuevo);

            // Enviar correo electrónico de verificación
            string subjectNuevo = "Verificación de registro";
            string bodyNuevo = $"Gracias por registrarte en Proyecto Foo. Por favor, utiliza el siguiente código para verificar tu cuenta: {verificationCodeNuevo}. Este código expirará en 15 minutos.";

            try
            {
                await _emailService.SendEmailAsync(createdUser.Email, subjectNuevo, bodyNuevo);
                _logger.LogInformation("Correo de verificación enviado a {email}", createdUser.Email);
                return new CreateUserResponse { Success = true, Message = "Cuenta creada. Por favor, verifica tu correo electrónico." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo de verificación al usuario con ID {createdUser.Id} y correo {createdUser.Email}.", createdUser.Id, createdUser.Email);
                // Considerar revertir la creación del usuario
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
