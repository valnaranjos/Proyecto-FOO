using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients
{
    public class GetAllPatientsResponse
    {
        public List<PatientDTO>? Patients { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }

    }
}
