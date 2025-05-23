using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProyectoFoo.Domain.Common.Enums;
using ProyectoFoo.Shared.ValidationAttributes;
namespace ProyectoFoo.Application.Features.Patients.CRUD
{
    public class CreatePatientCommand : IRequest<CreatePatientResponse>
    {
        [NotNullOrWhitespace(ErrorMessage = "El nombre es obligatorio y no puede contener solo espacios.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Name { get; set; } = string.Empty;

        [NotNullOrWhitespace(ErrorMessage = "El apellido es obligatorio y no puede contener solo espacios.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Surname { get; set; } = string.Empty;


        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [NotFutureDate(ErrorMessage = "La fecha de nacimiento no puede ser en el futuro.")]
        public DateTime Birthdate { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "La nacionalidad debe tener entre 4 y 30 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "La nacionalidad solo puede contener letras, acentos, la 'ñ' y espacios.")]
        public string Nationality { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de identifiación es obligatorio.")]
        [StringLength(50, ErrorMessage = "El tipo de identificación no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El tipo de identificación solo puede contener letras, acentos, la 'ñ' y espacios.")]
        public string TypeOfIdentification { get; set; } = string.Empty;

        [Required(ErrorMessage = "La identificación del paciente es obligatoria.")]
        [StringLength(20, MinimumLength = 9, ErrorMessage = "La identificación debe tener entre 9 y 20 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "La identificación solo puede contener letras, números y guiones.")]
        public string Identification { get; set; } = string.Empty;


        [Required(ErrorMessage = "El sexo es obligatorio.")]
        public SexType Sex { get; set; } = SexType.Masculino;


        [Required(ErrorMessage = "El correo electrónico del paciente es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        public string Email { get; set; } = string.Empty;


        [Phone(ErrorMessage = "Número de teléfono no válido.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El número de móvil no puede exceder los 20 caracteres, ni ser menor a 6 caracteres.")]
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

        public string? SessionDay { get; set; }

        public ModalityType? Modality { get; set; }
        public int? SessionDuration { get; set; }

        public string? SessionFrequency { get; set; }

        public string? PreferedContact { get; set; }

        // **NUEVA PROPIEDAD: Para el UserId del psicólogo autenticado**
        // Este valor NO viene del JSON, sino del controlador
        public int UserId { get; set; } 
    }
}
