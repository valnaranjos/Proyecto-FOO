using ProyectoFoo.Application.Features.Patients;
using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.API.Models.Dtos
{
    public class PacienteRequestDto
    {
        public PatientDTO NuevoPaciente { get; set; }

        public PacienteRequestDto(PatientDTO nuevoPaciente)
        {
            NuevoPaciente = nuevoPaciente ?? throw new ArgumentNullException(nameof(nuevoPaciente));
        }

        public PacienteRequestDto()
        {
            NuevoPaciente = new PatientDTO(); 
        }
    }
}
