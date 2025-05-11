using System;
using System.ComponentModel.DataAnnotations;
using ProyectoFoo.Domain.Common.Enums;

namespace ProyectoFoo.Domain.Entities
{
    public class Paciente
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El  nombre de usuario solo puede contener letras y espacios.")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El apellido del usuario no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El  apellido del usuario solo puede contener letras y espacios.")]
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

        public ModalityType Modality { get; set; }

        public DateTimeOffset AdmissionDate { get; set; } = DateTime.UtcNow;

        public int Age { get; set; }

        public string AgeRange { get; set; } = string.Empty;

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
        }

        public Paciente(string name, string surname, DateTime birthdate, string nationality, string typeofidentification, string identification, SexType sex, string email, string phone)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Surname = surname ?? throw new ArgumentNullException(nameof(surname));
            Birthdate = birthdate;
            Nationality = nationality ?? throw new ArgumentNullException(nameof(nationality)); ;
            TypeOfIdentification = typeofidentification ?? throw new ArgumentNullException(nameof(typeofidentification));
            Identification = identification ?? throw new ArgumentNullException(nameof(identification));
            Sex = sex;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Age = CalculateAge(birthdate);
            AgeRange = CalculateAgeRange(Age);
            AdmissionDate = DateTime.UtcNow;            
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

        private SexType ParseSex(string sex)
        {
            if (Enum.TryParse<SexType>(sex, true, out var result))
                return result;
            throw new ArgumentException($"Valor de sexo inválido: {sex}");
        }

        private ModalityType ParseModality(string modality)
        {
            if (Enum.TryParse<ModalityType>(modality, true, out var result))
                return result;
            throw new ArgumentException($"Valor de modalidad inválido: {modality}");
        }
    }
}
