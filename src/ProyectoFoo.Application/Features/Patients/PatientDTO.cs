using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoFoo.Application.Common.Enums;

namespace ProyectoFoo.Application.Features.Patients
{
    
   public class PatientDTO
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public DateTime Birthdate { get; set; }
    public int Identification { get; set; }
  
    public SexType Sex { get; set; } = SexType.Masculino;
    public string Modality { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
  
    // Propiedades adicionales para respuesta
    public int Age { get; set; }
    public DateTimeOffset AdmissionDate { get; set; }
    public string RangoEtario { get; set; } = string.Empty;
  }
}
