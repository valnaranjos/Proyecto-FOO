using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.Application.Features.Patients
{
    public class DeletePatientCommand : IRequest<DeletePatientResponse>
    {
        [Required(ErrorMessage = "El ID del paciente es obligatorio para la eliminación.")]
        public int PatientId { get; set; }
    }
}