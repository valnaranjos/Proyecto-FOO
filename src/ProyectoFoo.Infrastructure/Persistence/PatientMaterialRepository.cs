using Microsoft.EntityFrameworkCore;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Infrastructure.Persistence
{
    public class PatientMaterialRepository : BaseRepository<PatientMaterial>,  IPatientMaterialRepository
    {
        private new readonly ApplicationContextSqlServer _dbContext;

        public PatientMaterialRepository(ApplicationContextSqlServer dbContext) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<PatientMaterial>> GetByPatientIdAsync(int pacienteId)
        {
            return await _dbContext.PatientMaterials
                .Where(pm => pm.PatientId == pacienteId)
                .ToListAsync();
        }
    }
}
