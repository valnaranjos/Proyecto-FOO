using Microsoft.EntityFrameworkCore;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Infrastructure.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Infrastructure.Persistence
{
    public class PatientNoteRepository : IPatientNoteRepository
    {
        private readonly ApplicationContextSqlServer _context;

        public PatientNoteRepository(ApplicationContextSqlServer context)
        {
            _context = context;
        }

        public async Task AddAsync(Note note, CancellationToken cancellationToken)
        {
            await _context.Notes.AddAsync(note, cancellationToken);
        }

        public async Task DeleteAsync(Note note, CancellationToken cancellationToken)
        {
            _context.Notes.Remove(note);
            await Task.CompletedTask;
        }

        public async Task<List<Note>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Notes
                                 .AsNoTracking()
                                 .ToListAsync(cancellationToken);
        }

        public IQueryable<Note> GetAll()
        {
            return _context.Notes.AsNoTracking();
        }

        public async Task<Note?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Notes.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Note>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken)
{
    return await _context.Notes
        .Where(n => n.PatientId == patientId)
        .ToListAsync(cancellationToken);
}
    }
}
