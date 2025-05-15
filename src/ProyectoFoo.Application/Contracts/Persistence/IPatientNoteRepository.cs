using ProyectoFoo.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IPatientNoteRepository : IAsyncRepository<PatientNote>
    {
       Task<List<PatientNote>> GetPatientByIdAsync(int patientId);
      
        Task<List<PatientNote>> GetNotesByPatientIdAsync(int patientId);

        Task<PatientNote?> GetByIdAsync(int noteId, CancellationToken cancellationToken);
    }
}
