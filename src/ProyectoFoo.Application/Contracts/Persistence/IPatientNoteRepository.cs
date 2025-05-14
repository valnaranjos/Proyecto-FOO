using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IPatientNoteRepository
    {
        IQueryable<Note> GetAll();
        Task<Note?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(Note note, CancellationToken cancellationToken);
        void Remove(Note note);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}