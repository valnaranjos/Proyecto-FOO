using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace ProyectoFoo.Application.Features.Patients
{
    public class GetPatientByIdQuery : IRequest<GetPatientByIdResponse>
    {
        public int PatientId { get; set; }

        public GetPatientByIdQuery(int patientId)
        {
            PatientId = patientId;
        }
    }
}
