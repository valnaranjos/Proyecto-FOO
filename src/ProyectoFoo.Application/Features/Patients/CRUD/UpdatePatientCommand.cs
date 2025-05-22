using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoFoo.Domain.Common.Enums;
namespace ProyectoFoo.Application.Features.Patients.CRUD
{
    public class UpdatePatientCommand : IRequest<UpdatePatientResponse>
    {
        [Required(ErrorMessage = "El ID del paciente es obligatorio para la actualización.")]
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateTime? Birthdate { get; set; }

        public string? TypeOfIdentification { get; set; }
        public string? Identification { get; set; } 

        public SexType? Sex { get; set; } = SexType.Masculino;

        [EmailAddress(ErrorMessage = "Formato de correo electrónico no válido.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Formato de número de teléfono no válido.")]
        public string? Phone { get; set; }

        public string? Nationality { get; set; } = string.Empty;


        //OPCIONALES

        //Motivo de consulta

        public string? PrincipalMotive { get; set; }

        public string? ActualSymptoms { get; set; }

        public string? RecentEvents { get; set; }

        public string? PreviousDiagnosis { get; set; }


        //Historia clinica

        public string? ProfesionalObservations { get; set; }
        public string? KeyWords { get; set; }

        public string? FailedActs { get; set; }

        public string? Interconsulation { get; set; }

        public string? PatientEvolution { get; set; }



        //Organizacion y seguimiento

        public DateTime? SessionDay { get; set; }

        public ModalityType? Modality { get; set; }
        public int? SessionDuration { get; set; }

        public string? SessionFrequency { get; set; }

        public string? PreferedContact { get; set; }


        public int UserId { get; set; }
    }
}
