using ProyectoFoo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IPatientRepository : IAsyncRepository<Paciente>
    {
        //Como hereda, no se deben implementar los métodos de la interfaz IAsyncRepository<T>
        // Task<List<Paciente>> FindByAsync(Expression<Func<Paciente, bool>> predicate); para buscar por nombre, o matrícula (cedula/identificacion)
    }
}
