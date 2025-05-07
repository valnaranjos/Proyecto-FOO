using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoFoo.Application.Common.Enums;
namespace ProyectoFoo.Application.Features.Patients
{
    public class UpdatePatientCommand : IRequest<UpdatePatientResponse>
    {
        [Required(ErrorMessage = "El ID del paciente es obligatorio para la actualización.")]
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateTime? Birthdate { get; set; }
        public int? Identification { get; set; }
        public SexType Sex { get; set; } = SexType.Masculino;
        public string? Modality { get; set; }

        [EmailAddress(ErrorMessage = "Formato de correo electrónico no válido.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Formato de número de teléfono no válido.")]
        public string? Phone { get; set; }

    }
}
