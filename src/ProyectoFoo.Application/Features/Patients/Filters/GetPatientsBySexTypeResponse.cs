using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Filters
{
    public class GetPatientsBySexTypeResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<PatientDTO>? Patients { get; set; }
    }
}
