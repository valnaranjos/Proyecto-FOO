using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Search
{
    public class GetPatientByIdentificationCommand : IRequest<GetPatientByIdResponse>
    {
        public string Identification { get; set; }
        public GetPatientByIdentificationCommand(string identification)
        {
            Identification = identification;
        }
    }
}
