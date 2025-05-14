using MediatR;
using ProyectoFoo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Users
{
    public class GetCurrentUserCommand : IRequest<UserInfoDto>
    {
        /// <summary>
        /// ID del usuario autenticado.
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la consulta GetCurrentUserQuery.
        /// </summary>
        /// <param name="userId">ID del usuario autenticado.</param>
        public GetCurrentUserCommand(int userId)
        {
            UserId = userId;
        }
    }
}
