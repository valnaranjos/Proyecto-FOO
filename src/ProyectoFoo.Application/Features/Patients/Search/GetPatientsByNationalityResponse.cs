using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Search
{
    public class GetPatientsByNationalityResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<PatientDTO>? Patients { get; set; }
    }
}
