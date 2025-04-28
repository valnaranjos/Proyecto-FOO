using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Shared.Models
{
    public class Paciente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios.")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateOnly Birthdate { get; set; }

        [Required(ErrorMessage = "El número de identificación es obligatorio.")]
        [Range(10, int.MaxValue, ErrorMessage = "El número de identificación debe ser positivo.")]
        public int Identification { get; set; }

        //(F/M/O)
        [Required(ErrorMessage = "El sexo es obligatorio.")]
        [RegularExpression(@"^(F|M|O)$", ErrorMessage = "Sexo inválido. Usa F, M u O.")]
        [StringLength(1, ErrorMessage = "El sexo debe tener 1 carácter: F/M/O")] //Ejemplo adicional
        public string Sex { get; set; } = string.Empty;

       
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")] // Ejemplo adicional
        public string Email { get; set; }


        [Phone(ErrorMessage = "Número de teléfono no válido.")]
        [StringLength(20, ErrorMessage = "El número de teléfono no puede exceder los 20 caracteres.")] // Ejemplo adicional
        public string Phone { get; set; }

        //Virtual/Presencial
        [Required(ErrorMessage = "La modalidad es obligatoria.")]
        [RegularExpression(@"^(Presencial|Virtual)$", ErrorMessage = "Las opciones son 'Presencial' o 'Virtual'.")]
        public string Modality { get; set; } = string.Empty;

        //Se calcula automática
        public DateTimeOffset AdmissionDate { get; set; } = DateTime.UtcNow;


        //No obligatorios
        public string? Diagnosis { get; set; }
        public string? Institution { get; set; }

        // // Constructor sin parámetros (requerido por EF Core) para instanciar las entidades al leer la base de datos.
        public Paciente()
        {

        }
        public Paciente(int id, string name, string surname, DateTime birthdate, int identification, string sex, string modality, string email, string phone)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Birthdate = birthdate;
            Identification = identification;
            Sex = sex;
            Modality = modality;
            Email = email;
            Phone = phone;
            AdmissionDate = DateTime.UtcNow;
        }

        public Paciente(int id, string name, string surname, DateTime birthdate, int identification, string sex, string modality, string diagnosis, string institution, string email, string phone)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Birthdate = birthdate;
            Identification = identification;
            Sex = sex;
            Modality = modality;
            Diagnosis = diagnosis;
            Institution = institution;
            Email = email;
            Phone = phone;
            AdmissionDate = DateTime.UtcNow;
        }

        [NotMapped]
        public int Age => DateTime.Today.Year - Birthdate.Year - (DateTime.Today.DayOfYear < Birthdate.DayOfYear ? 1 : 0);

    }
}
