using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Archive
{
    public class GetAllArchivedPatientsCommand : IRequest<List<PatientDTO>>
    {
        public int UserId { get; }

        public GetAllArchivedPatientsCommand(int userId)
        {
            UserId = userId;
        }
    }
}
