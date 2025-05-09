using System;
using System.ComponentModel.DataAnnotations;
using ProyectoFoo.Domain.Common.Enums;

namespace ProyectoFoo.Domain.Entities
{
    public class Paciente
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public DateTime Birthdate { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El tipo de identificación no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El tipo de identificación solo puede contener letras y espacios.")]
        public string TypeOfIdentification { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "La identificación debe tener hasta 20 dígitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La identificación solo puede contener números.")]
        public string Identification { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "La nacionalidad no puede exceder los 30 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "La nacionalidad solo puede contener letras y espacios.")]
        public string Nationality { get; set; }

        [Required]
        public SexType Sex { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        [Required]
        public ModalityType Modality { get; set; }

        public DateTimeOffset AdmissionDate { get; set; } = DateTime.UtcNow;

        public int Age { get; set; }

        public string RangoEtario { get; set; } = string.Empty;

        public Paciente()
        {
            Name = string.Empty;
            Surname = string.Empty;
            Nationality = string.Empty;
            Sex = SexType.Masculino;
            Modality = ModalityType.Presencial; // Valor por defecto (ajústalo si es necesario)
            Identification = string.Empty;
        }

        public Paciente(string name, string surname, DateTime birthdate, string identification, string sex, string modality, string email, string phone)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Surname = surname ?? throw new ArgumentNullException(nameof(surname));
            Birthdate = birthdate;
            Identification = identification ?? throw new ArgumentNullException(nameof(identification));
            Sex = ParseSex(sex);
            Modality = ParseModality(modality);
            Email = email;
            Phone = phone;
            AdmissionDate = DateTime.UtcNow;

            CalcularEdadYRangoEtario();
        }

        public Paciente(int id, string name, string surname, DateTime birthdate, string identification, string sex, string modality, string diagnosis, string email, string phone)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Surname = surname ?? throw new ArgumentNullException(nameof(surname));
            Birthdate = birthdate;
            Identification = identification ?? throw new ArgumentNullException(nameof(identification));
            Sex = ParseSex(sex);
            Modality = ParseModality(modality);
            Email = email;
            Phone = phone;
            AdmissionDate = DateTime.UtcNow;

            CalcularEdadYRangoEtario();
        }

        private void CalcularEdadYRangoEtario()
        {
            var today = DateTime.UtcNow;
            Age = today.Year - Birthdate.Year;
            if (Birthdate > today.AddYears(-Age)) Age--;

            if (Age <= 12)
                RangoEtario = "Niño";
            else if (Age < 18)
                RangoEtario = "Adolescente";
            else
                RangoEtario = "Adulto";
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
