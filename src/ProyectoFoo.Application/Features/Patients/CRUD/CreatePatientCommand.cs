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
    public class CreatePatientCommand : IRequest<CreatePatientResponse>
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido del usuario no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El Apellido solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Surname { get; set; } = string.Empty;


        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime Birthdate { get; set; }

        [Required(ErrorMessage = "La nacionalidad es obligatoria.")]
        [StringLength(30, ErrorMessage = "La nacionalidad no puede exceder los 30 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "La nacionalidad solo puede contener letras y espacios.")]
        public string Nationality { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de identifiación es obligatorio.")]
        [StringLength(50, ErrorMessage = "El tipo de identificación no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El tipo de identificación solo puede contener letras y espacios.")]
        public string TypeOfIdentification {get; set;} = string.Empty;

        [Required]
        [StringLength(20, ErrorMessage = "La identificación debe tener hasta 20 dígitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La identificación solo puede contener números.")]
        public string Identification { get; set; } = string.Empty;


        [Required(ErrorMessage = "El sexo es obligatorio.")]
        public SexType Sex { get; set; } = SexType.Masculino;

       
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico no válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [Phone(ErrorMessage = "Formato de número de teléfono no válido.")]
        public string Phone { get; set; } = string.Empty;


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

        public int UserId { get; set; } = 0;
    }
}
