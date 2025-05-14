using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Search
{
    public class GetPatientsByNationalityCommand : IRequest<GetPatientsByNationalityResponse>
    {
        public string Nationality { get; set; }

        public GetPatientsByNationalityCommand(string nationality)
        {
            Nationality = nationality;
        }
    }
}
