using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients
{
    public class UpdatePatientHandler : IRequestHandler<UpdatePatientCommand, UpdatePatientResponse>
    {
        private readonly IPatientRepository _pacienteRepository;

        public UpdatePatientHandler(IPatientRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<UpdatePatientResponse> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var patientToUpdate = await _pacienteRepository.GetByIdAsync(request.Id);

            if (patientToUpdate == null)
            {
                return new UpdatePatientResponse
                {
                    Success = false,
                    Message = $"Paciente con ID {request.Id} no encontrado."
                };
            }

            // Actualizar solo las propiedades que tienen un valor en la request
            if (request.Name != null) patientToUpdate.Name = request.Name;
            if (request.Surname != null) patientToUpdate.Surname = request.Surname;
            if (request.Birthdate.HasValue) patientToUpdate.Birthdate = request.Birthdate.Value;
            if (request.Identification.HasValue) patientToUpdate.Identification = request.Identification.Value;
            if (request.Sex != null) patientToUpdate.Sex = request.Sex;
            if (request.Email != null) patientToUpdate.Email = request.Email;
            if (request.Phone != null) patientToUpdate.Phone = request.Phone;
            if (request.Modality != null) patientToUpdate.Modality = request.Modality;

            await _pacienteRepository.UpdateAsync(patientToUpdate);

            return new UpdatePatientResponse
            {
                PatientId = patientToUpdate.Id,
                Success = true
            };
        }
    }
}
