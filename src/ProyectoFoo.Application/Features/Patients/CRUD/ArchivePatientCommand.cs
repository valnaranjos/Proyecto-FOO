using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.CRUD
{
    public class ArchivePatientCommand : IRequest<ArchivePatientResponse>
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public ArchivePatientCommand(int id, int userId)
        {
            Id = id;
            UserId = userId;
        }
    }
}
