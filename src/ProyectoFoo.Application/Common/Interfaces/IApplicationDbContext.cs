using Microsoft.EntityFrameworkCore;
using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Note> Notes { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
