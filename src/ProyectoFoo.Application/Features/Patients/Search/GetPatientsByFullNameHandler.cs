using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.ServiceExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Search
{
    public class GetPatientsByFullNameHandler : IRequestHandler<GetPatientsByFullNameCommand, GetPatientsByFullNameResponse>
    {
        private readonly IPatientRepository  _patientRepository;

        public GetPatientsByFullNameHandler(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        }
         
        public async Task<GetPatientsByFullNameResponse> Handle(GetPatientsByFullNameCommand request, CancellationToken cancellationToken)
        {
            var fullNameLower = request.FullName.ToLower();

            var patients = await _patientRepository.ListPatientsAsync(p =>
                (p.Name.ToLower() + " " + p.Surname.ToLower()).Contains(fullNameLower)
            );

            if (patients == null || !patients.Any())
            {
                return new GetPatientsByFullNameResponse
                {
                    Success = false,
                    Message = $"No se encontraron pacientes con el nombre: {request.FullName}"
                };
            }

            var patientDtos = patients.Select(patient => new PatientDTO
            {
                 Id = patient.Id,
                Name = patient.Name.CapitalizeFirstLetter(),
                Surname = patient.Surname.CapitalizeFirstLetter(),
                Birthdate = patient.Birthdate,
                Identification = patient.Identification,
                TypeOfIdentification = patient.TypeOfIdentification?.ToUpper() ?? string.Empty,
                Sex = patient.Sex,
                Modality = patient.Modality,
                Email = patient.Email ?? string.Empty,
                Phone = patient.Phone ?? string.Empty,
                Age = patient.CalculateAge(patient.Birthdate),
                RangoEtario = patient.AgeRange, // Ya es string en la entidad
                Nationality = patient.Nationality.CapitalizeFirstLetter(),
                AdmissionDate = patient.AdmissionDate,
            }).ToList();

            return new GetPatientsByFullNameResponse
            {
                Success = true,
                Patients = patientDtos
            };
        }
    }
}
