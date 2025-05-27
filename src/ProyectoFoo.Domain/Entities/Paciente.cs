using System;
using System.ComponentModel.DataAnnotations;
using ProyectoFoo.Domain.Common.Enums;
using ProyectoFoo.Shared.ValidationAttributes;

namespace ProyectoFoo.Domain.Entities
{
    public class Paciente
    {
        //OBLIGATORIAS
        public int Id { get; set; }
        


        [NotNullOrWhitespace(ErrorMessage = "El nombre es obligatorio y no puede contener solo espacios.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Name { get; set; } = string.Empty;

        [NotNullOrWhitespace(ErrorMessage = "El apellido es obligatorio y no puede contener solo espacios.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Surname { get; set; } = string.Empty;


        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [NotFutureDate(ErrorMessage = "La fecha de nacimiento no puede ser en el futuro.")]
        public DateTime Birthdate { get; set; }


        [Required]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "La nacionalidad debe tener entre 4 y 30 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "La nacionalidad solo puede contener letras, acentos, la 'ñ' y espacios.")]
        public string Nationality { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El tipo de identificación no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El tipo de identificación solo puede contener letras, acentos, la 'ñ' y espacios.")]
        public string TypeOfIdentification { get; set; }


        [Required(ErrorMessage = "El número de identificación es obligatorio.")]
        [StringLength(20, MinimumLength = 9, ErrorMessage = "La identificación debe tener entre 9 y 20 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "La identificación solo puede contener letras, números y guiones.")]
        public string Identification { get; set; } = string.Empty;


        [Required]
        public SexType Sex { get; set; }


        [Required(ErrorMessage = "El correo electrónico del paciente es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El número móvil del paciente es obligatorio.")]
        [Phone(ErrorMessage = "Número de teléfono no válido.")]
        [StringLength(20, MinimumLength =6, ErrorMessage = "El número de móvil no puede exceder los 20 caracteres, ni ser menor a 6 caracteres.")]
        public string Phone { get; set; }

        //CALCULADAS O AUTOMÁTICAS
        public DateTimeOffset AdmissionDate { get; set; } = DateTime.UtcNow;

        public int Age { get; set; }

        public string AgeRange { get; set; } = string.Empty;

        public bool IsEnabled { get; set; } = true;



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


        //Relaciones con notas y materiales
        public ICollection<PatientNote> Notes { get; set; } = [];
        public ICollection<PatientMaterial> Materials { get; set; } = [];


        //Relación con Usuario (psicologo)
        public int? UserId { get; set; }
        public Usuario User { get; set; } = null!;



        //Constructor por defecto
        public Paciente()
        {
            Name = string.Empty;
            Surname = string.Empty;
            Nationality = string.Empty;
            Sex = SexType.Masculino;
            Modality = ModalityType.Presencial;
            Identification = string.Empty;
            TypeOfIdentification = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            IsEnabled = true;
        }

        // Constructor con UserId
        public Paciente(int userId) : this()
        {
            UserId = userId;
        }

        public string CalculateAgeRange(int age)
        {
            if (Age <= 12)
                AgeRange = "Niño";
            else if (Age < 18)
                AgeRange = "Adolescente";
            else
                AgeRange = "Adulto";

            return AgeRange;        
        }

        public int CalculateAge(DateTime birthday)
        {           
            var today = DateTime.UtcNow;
            Age = today.Year - Birthdate.Year;
            if (Birthdate > today.AddYears(-Age)) Age--;
            return Age;      
        }

    }
}
