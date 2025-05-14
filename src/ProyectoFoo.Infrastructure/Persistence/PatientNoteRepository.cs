using Microsoft.EntityFrameworkCore;
using ProyectoFoo.Application.Common.Interfaces.Repositories;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Infrastructure.Context;

namespace ProyectoFoo.Infrastructure.Repositories
{
    public class PatientNoteRepository : IPatientNoteRepository
    {
        private readonly ApplicationContextSqlServer _context;

        public PatientNoteRepository(ApplicationContextSqlServer context)
        {
            _context = context;
        }

        public IQueryable<Note> GetAll()
        {
            return _context.Notes.AsNoTracking();
        }

        public async Task<Note?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Notes.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task AddAsync(Note note, CancellationToken cancellationToken)
        {
            await _context.Notes.AddAsync(note, cancellationToken);
        }

        public void Remove(Note note)
        {
            _context.Notes.Remove(note);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}