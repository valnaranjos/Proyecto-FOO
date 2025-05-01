using MediatR;
using System.Collections.Generic;

namespace ProyectoFoo.Application.Features.Patients
{
    public class GetAllPatientsQuery : IRequest<GetAllPatientsResponse>
    {
    }
}
