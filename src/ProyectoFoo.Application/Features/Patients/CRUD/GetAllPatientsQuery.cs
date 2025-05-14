using MediatR;
using System.Collections.Generic;

namespace ProyectoFoo.Application.Features.Patients.CRUD
{
    public class GetAllPatientsQuery : IRequest<GetAllPatientsResponse>
    {
        public string? TypeOfIdentification { get; set; }
    }
}
