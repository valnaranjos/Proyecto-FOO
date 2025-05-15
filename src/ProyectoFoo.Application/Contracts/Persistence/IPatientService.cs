using ProyectoFoo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    /// <summary>
    /// Servicio para la gestión de búsqueda y filtro de pacientes.
    /// </summary>
    public interface IPatientService
    {
        Task<Paciente> GetPatientByEmailAsync(string email);
        Task<List<Paciente>> GetPacientesByModalityAsync(string modality);
    }
}
