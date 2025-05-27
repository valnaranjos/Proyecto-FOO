using MediatR;
using ProyectoFoo.Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Search
{
    public record SearchPatientsQuery(
    string? Identification,
    string? FullName,
    string? Email,
    string? Nationality,
    SexType? SexType,
    ModalityType? Modality,
    string? AgeRange
) : IRequest<IEnumerable<PatientDTO>>;
    
}
