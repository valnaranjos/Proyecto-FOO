using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.CRUD
{
    public class UnarchivePatientCommand : IRequest<UnarchivePatientResponse>
    {
        public int PatientId { get; }

        public UnarchivePatientCommand(int patientId)
        {
            PatientId = patientId;
        }
    }
}
