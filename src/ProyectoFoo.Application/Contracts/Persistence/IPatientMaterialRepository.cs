using ProyectoFoo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IPatientMaterialRepository : IAsyncRepository<PatientMaterial>
    {
        Task<List<PatientMaterial>> GetByPatientIdAsync(int pacienteId);
    }
}
