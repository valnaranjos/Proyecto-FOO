using ProyectoFoo.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IPatientNoteRepository
    {
        Task AddAsync(Note note, CancellationToken cancellationToken);
        Task<Note?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<Note>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<Note>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken);
        Task DeleteAsync(Note note, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
