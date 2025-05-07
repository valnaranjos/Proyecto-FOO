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
    public class CreatePatientCommand : IRequest<CreatePatientResponse>
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Surname { get; set; } = string.Empty;

        public DateTime Birthdate { get; set; }

        [Required(ErrorMessage = "El número de identificación es obligatorio.")]
        public int Identification { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio.")]
        public SexType Sex { get; set; } = SexType.Masculino;

        [Required(ErrorMessage = "La modalidad es obligatoria.")]
        public string Modality { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")] // Ahora Email es obligatorio
        [EmailAddress(ErrorMessage = "Formato de correo electrónico no válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")] // Ahora Phone es obligatorio
        [Phone(ErrorMessage = "Formato de número de teléfono no válido.")]
        public string Phone { get; set; } = string.Empty;


    }
}
