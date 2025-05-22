using System;
using System.ComponentModel.DataAnnotations;
using ProyectoFoo.Domain.Common.Enums;

namespace ProyectoFoo.Domain.Entities
{
    public class Paciente
    {
        //OBLIGATORIAS
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El apellido del usuario no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El Apellido solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Surname { get; set; }

        [Required]
        public DateTime Birthdate { get; set; }


        [Required]
        [StringLength(30, ErrorMessage = "La nacionalidad no puede exceder los 30 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "La nacionalidad solo puede contener letras y espacios.")]
        public string Nationality { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El tipo de identificación no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El tipo de identificación solo puede contener letras y espacios.")]
        public string TypeOfIdentification { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "La identificación debe tener hasta 20 dígitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La identificación solo puede contener números.")]
        public string Identification { get; set; }


        [Required]
        public SexType Sex { get; set; }


        [Required]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico no válido.")]
        public string Email { get; set; }

        [Required]
        [Phone(ErrorMessage = "Formato de número de teléfono no válido.")]
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

        public DateTime? SessionDay { get; set; }

        public ModalityType? Modality { get; set; }
        public int? SessionDuration { get; set; }

        public string? SessionFrequency { get; set; }

        public string? PreferedContact { get; set; }


        //Relaciones con notas y materiales
        public ICollection<PatientNote> Notes { get; set; } = new List<PatientNote>();
        public ICollection<PatientMaterial> Materials { get; set; } = new List<PatientMaterial>();


        //Relación con Usuario (psicologo)
        public int? UserId { get; set; }
        public Usuario User { get; set; } = null!;



        //Constructor por defecto
        public Paciente()
        {
            Name = string.Empty;
            Surname = string.Empty;
            Nationality = string.Empty;
            Sex = SexType.Masculino; // Valor por defecto
            Modality = ModalityType.Presencial; // Valor por defecto
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
