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
        public int Identification { get; set; }

        [Required]
        public SexType Sex { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        [Required]
        public string Modality { get; set; }

        public DateTimeOffset AdmissionDate { get; set; } = DateTime.UtcNow;

        public int Age { get; set; }

        public string RangoEtario { get; set; } = string.Empty;

        // Constructor requerido por EF Core
        public Paciente()
        {
            Name = string.Empty;
            Surname = string.Empty;
            Sex = SexType.Masculino;
            Modality = string.Empty;
        }

        public Paciente(string name, string surname, DateTime birthdate, int identification, string sex, string modality, string email, string phone)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Surname = surname ?? throw new ArgumentNullException(nameof(surname));
            Birthdate = birthdate;
            Identification = identification;
            Sex = ParseSex(sex); 
            Modality = modality ?? throw new ArgumentNullException(nameof(modality));
            Email = email;
            Phone = phone;
            AdmissionDate = DateTime.UtcNow;

            CalcularEdadYRangoEtario();
        }

        public Paciente(int id, string name, string surname, DateTime birthdate, int identification, string sex, string modality, string diagnosis, string email, string phone)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Surname = surname ?? throw new ArgumentNullException(nameof(surname));
            Birthdate = birthdate;
            Identification = identification;
            Sex = ParseSex(sex);
            Modality = modality ?? throw new ArgumentNullException(nameof(modality));
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
    }
}
