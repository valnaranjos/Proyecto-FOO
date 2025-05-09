using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public string Identification { get; set; }
  
    public SexType Sex { get; set; } = SexType.Masculino;
    public ModalityType Modality { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
  
    public int Age { get; set; }
    public DateTimeOffset AdmissionDate { get; set; }
    public string RangoEtario { get; set; } = string.Empty;
  }
}
