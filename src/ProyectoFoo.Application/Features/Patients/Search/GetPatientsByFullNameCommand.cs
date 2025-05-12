using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Search
{
    public class GetPatientsByFullNameCommand : IRequest<GetPatientsByFullNameResponse>
    {
        public string FullName { get; }
        public GetPatientsByFullNameCommand(string fullName)
        {
            FullName = fullName;
        }
    }
}
