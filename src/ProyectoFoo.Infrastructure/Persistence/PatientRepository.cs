using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Infrastructure.Context;

namespace ProyectoFoo.Infrastructure.Persistence
{
    public class PatientRepository : BaseRepository<Paciente>, IPatientRepository
    {
        private readonly ApplicationContextSqlServer _dbContext;

        // Constructor que recibe el DbContext y lo pasa a la base
        public PatientRepository(ApplicationContextSqlServer dbContext) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        //Como hereda de BaseRepository, no es necesario implementar los métodos de la interfaz IPatientRepository
    }
}
