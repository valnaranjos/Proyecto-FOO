using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients
{
    public class CreatePatientHandler : IRequestHandler<CreatePatientCommand, CreatePatientResponse>
    {
        private readonly IPatientRepository _pacienteRepository;

        public CreatePatientHandler(IPatientRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<CreatePatientResponse> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            var paciente = new Paciente
            {
                Name = request.Name,
                Surname = request.Surname,
                Birthdate = request.Birthdate,
                Identification = request.Identification,
                Sex = request.Sex,
                Modality = request.Modality,
                Email = request.Email,
                Phone = request.Phone,
                Diagnosis = request.Diagnosis,
                Institution = request.Institution
            };

            var newPaciente = await _pacienteRepository.AddAsync(paciente);

            return new CreatePatientResponse
            {
                PatientId = newPaciente.Id,
                Success = true
            };
        }
    }
}
