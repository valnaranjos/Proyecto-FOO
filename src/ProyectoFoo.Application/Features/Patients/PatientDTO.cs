using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using ProyectoFoo.Domain.Common.Enums;

namespace ProyectoFoo.Application.Features.Patients
{

    public class PatientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }

        public string TypeOfIdentification { get; set; } = string.Empty;
        public string Identification { get; set; } = string.Empty;

        public SexType Sex { get; set; } = SexType.Masculino;
        
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public int Age { get; set; }
        public DateTimeOffset AdmissionDate { get; set; }
        public string RangoEtario { get; set; } = string.Empty;

        public string Nationality { get; set; } = string.Empty;


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
    }
}
