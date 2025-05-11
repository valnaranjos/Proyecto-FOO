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
            if (!string.IsNullOrEmpty(request.Name)) patientToUpdate.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Surname)) patientToUpdate.Surname = request.Surname;

            if (request.Birthdate.HasValue) patientToUpdate.Birthdate = request.Birthdate.Value;
            if (!string.IsNullOrEmpty(request.Identification)) patientToUpdate.Identification = request.Identification;
            if (!string.IsNullOrEmpty(request.TypeOfIdentification)) patientToUpdate.TypeOfIdentification = request.TypeOfIdentification;

            if (!string.IsNullOrEmpty(request.Email)) patientToUpdate.Email = request.Email;
            if (!string.IsNullOrEmpty(request.Phone)) patientToUpdate.Phone = request.Phone;

            if (request.Nationality != null) patientToUpdate.Nationality = request.Nationality;

            try
            {
                await _pacienteRepository.UpdateAsync(patientToUpdate);
                return new UpdatePatientResponse { Success = true, Message = "Paciente actualizado exitosamente." };
            }
            catch (Exception ex)
            {
                return new UpdatePatientResponse { Success = false, Message = $"Error al actualizar el paciente: {ex.Message}" };
            }
        }
    }
}
