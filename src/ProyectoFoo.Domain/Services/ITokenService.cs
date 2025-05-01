using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Domain.Services
{
    public interface ITokenService
    {
        string GenerateToken(Usuario user);
    }
}
