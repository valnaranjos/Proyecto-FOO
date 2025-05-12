using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Filters
{
    public class GetPatientsByAgeRangeCommand : IRequest<GetPatientsByAgeRangeResponse>
    {
        public string AgeRange { get; set; }

        public GetPatientsByAgeRangeCommand(string ageRange)
        {
            AgeRange = ageRange;
        }
    }
}
