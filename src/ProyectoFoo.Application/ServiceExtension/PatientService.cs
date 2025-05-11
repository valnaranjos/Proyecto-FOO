using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.ServiceExtension
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository pacienteRepository)
        {
            _patientRepository = pacienteRepository;
        }

        public async Task<Paciente> GetPatientByEmailAsync(string email)
        {
            return await _patientRepository.GetByEmailAsync(email);
        }

        public async Task<List<Paciente>> GetPacientesByModalityAsync(string modality)
        {
            return await _patientRepository.GetByModalityAsync(modality);
        }
    }
}
