using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Application.Features.Users
{
    public class CreateUserResponse
    {
        public Usuario? User { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
