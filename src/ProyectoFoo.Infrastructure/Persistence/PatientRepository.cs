using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoFoo.Infrastructure.Persistence
{
    public class PatientRepository : BaseRepository<Paciente>, IPatientRepository
    {
        private readonly ApplicationContextSqlServer _dbContext;

        // Constructor que recibe el DbContext y lo pasa a la base
        public PatientRepository(ApplicationContextSqlServer dbContext) : base(dbContext)
        {
        }

        //Como hereda de BaseRepository, no es necesario implementar los métodos de la interfaz IPatientRepository
    }
}
