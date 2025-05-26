using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Users
{
    public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserCommand, UserInfoDto>
    {
        private readonly IUserRepository _userRepository;


        /// <summary>
        /// Inicializa una nueva instancia del handler GetCurrentUserHandler.
        /// </summary>
        /// <param name="usuarioRepository">Repositorio de usuarios.</param>
       
        public GetCurrentUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        /// Maneja la lógica para obtener el usuario autenticado.
        /// </summary>
        /// <param name="request">Consulta con el ID del usuario.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>DTO con los datos del usuario.</returns>
        public async Task<UserInfoDto> Handle(GetCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            return user == null
                ? throw new Exception("Usuario no encontrado.")
                : new UserInfoDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Phone = user.Phone,
                Title = user.Title,
            };
        }
    }
}
