using ProyectoFoo.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IPatientNoteRepository : IAsyncRepository<PatientNote>
    {
        Task<List<PatientNote>> GetByPatientIdAsync(int pacienteId);
    }
}
