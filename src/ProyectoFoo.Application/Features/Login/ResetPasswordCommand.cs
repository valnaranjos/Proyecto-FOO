using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.Application.Features.Login
{
    public class ResetPasswordCommand : IRequest<ResetPasswordResponse>
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        public required string Email { get; set; }
    }
}
