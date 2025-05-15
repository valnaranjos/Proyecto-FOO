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
    public class PatientNoteRepository : BaseRepository<PatientNote>, IPatientNoteRepository
    {
        private readonly ApplicationContextSqlServer _dbContext;

        public PatientNoteRepository(ApplicationContextSqlServer dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PatientNote>> GetPatientByIdAsync(int pacienteId)
        {
            return await _dbContext.PatientNotes
                .Where(pm => pm.PatientId == pacienteId)
                .ToListAsync();
        }

        public async Task<List<PatientNote>> GetNotesByPatientIdAsync(int patientId)
        {
            return await _dbContext.PatientNotes
                                 .Where(n => n.PatientId == patientId)
                                 .OrderByDescending(n => n.CreatedDate)
                                 .ToListAsync();
        }

        public async Task<PatientNote?> GetByIdAsync(int noteId, CancellationToken cancellationToken)
        {
            return await _dbContext.PatientNotes
                                 .FirstOrDefaultAsync(n => n.Id == noteId, cancellationToken);
        }
    }
}
