using ProyectoFoo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    public interface IPatientService
    {
        Task<Paciente> GetPatientByEmailAsync(string email);
        Task<List<Paciente>> GetPacientesByModalityAsync(string modality);
    }
}
