using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Application.Features.Users
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly IUserRepository _usuarioRepository;

        public CreateUserHandler(IUserRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Verificar si ya existe un usuario con la misma identificación
            if (await _usuarioRepository.ExistsAsync(request.Identification))
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = $"Ya existe un usuario con la identificación {request.Identification}."
                };
            }

            // Verificar si ya existe un usuario con el mismo correo electrónico
            var existingUserWithEmail = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (existingUserWithEmail != null)
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = $"Ya existe un usuario con el correo electrónico {request.Email}."
                };
            }

            // Crear la entidad Usuario (psicologo)
            var newUser = new Usuario(
                id: 0, // ID automatico (BD)
                nombre: request.Name,
                correo: request.Email,
                contrasena: request.Password // La contraseña se hashea en el constructor de Usuario
                )
            {
                Surname = request.Surname,
                Identification = request.Identification
            };

            // Agregar el nuevo usuario a la base de datos
            var createdUser = await _usuarioRepository.AddAsync(newUser);

            return new CreateUserResponse
            {
                Success = true,
                User = createdUser
            };
        }
    }
}
