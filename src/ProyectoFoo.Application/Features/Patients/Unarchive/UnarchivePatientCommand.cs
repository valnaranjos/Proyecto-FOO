﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Unarchive
{
    public class UnarchivePatientCommand : IRequest<UnarchivePatientResponse>
    {
        public int PatientId { get; }
        public int UserId { get; set; }

        public UnarchivePatientCommand(int patientId, int userId)
        {
            PatientId = patientId;
            UserId = userId;
        }
    }
}
