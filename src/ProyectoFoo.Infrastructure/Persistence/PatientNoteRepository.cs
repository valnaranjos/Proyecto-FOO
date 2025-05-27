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
        private new readonly ApplicationContextSqlServer _dbContext;

        public PatientNoteRepository(ApplicationContextSqlServer dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PatientNote>> GetByPatientIdAsync(int pacienteId)
        {
            return await _dbContext.PatientNotes
                .Where(pm => pm.PatientId == pacienteId)
                .ToListAsync();
        }
    }
}
