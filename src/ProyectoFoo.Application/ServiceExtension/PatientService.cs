using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.ServiceExtension
{
    public class PatientService(IPatientRepository pacienteRepository) : IPatientService
    {
        public async Task<Paciente> GetPatientByEmailAsync(string email)
        {
            return await pacienteRepository.GetByEmailAsync(email);
        }

        public async Task<List<Paciente>> GetPacientesByModalityAsync(string modality)
        {
            return await pacienteRepository.GetByModalityAsync(modality);
        }
    }
}
