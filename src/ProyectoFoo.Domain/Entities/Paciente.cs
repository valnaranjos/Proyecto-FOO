using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Domain.Entities
{
    public class Paciente
    {
        public int Id { get; set; }

       
        public string Name { get; set; }

        
        public string Surname { get; set; }


        public DateTime Birthdate { get; set; }

        
        public int Identification { get; set; }

       
        public string Sex { get; set; }

       
       
        public string? Email { get; set; }



        public string? Phone { get; set; }


        public string Modality { get; set; }

        //Se calcula automática
        public DateTimeOffset AdmissionDate { get; set; } = DateTime.UtcNow;


        //No obligatorios
        public string? Diagnosis { get; set; }
        public string? Institution { get; set; }

        // // Constructor sin parámetros (requerido por EF Core) para instanciar las entidades al leer la base de datos.
        public Paciente()
        {
            Name = string.Empty;
            Surname = string.Empty;
            Sex = string.Empty;
            Modality = string.Empty;
        }

        public Paciente( string name, string surname, DateTime birthdate, int identification, string sex, string modality, string email, string phone)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Surname = surname ?? throw new ArgumentNullException(nameof(surname));
            Birthdate = birthdate;
            Identification = identification;
            Sex = sex ?? throw new ArgumentNullException(nameof(sex)); ;
            Modality = modality ?? throw new ArgumentNullException(nameof(modality));
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
