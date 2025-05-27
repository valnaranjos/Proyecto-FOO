using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.Application.Features.Patients.CRUD.Delete
{
    public class DeletePatientCommand : IRequest<DeletePatientResponse>
    {
        [Required(ErrorMessage = "El ID del paciente es obligatorio para la eliminación.")]
        public int PatientId { get; set; }
        public int UserId { get; set; }

        public DeletePatientCommand()
        {
        }
    }
}