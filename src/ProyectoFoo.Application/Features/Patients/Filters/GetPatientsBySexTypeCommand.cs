using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Filters
{
    public class GetPatientsBySexTypeCommand : IRequest<GetPatientsBySexTypeResponse>
    {
        public string Sex { get; set; }

        public GetPatientsBySexTypeCommand(string sex)
        {
            Sex = sex;
        }
    }
}
