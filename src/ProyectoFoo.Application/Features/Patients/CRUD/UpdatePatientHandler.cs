using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.CRUD
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

            if (request.Sex.HasValue) patientToUpdate.Sex = request.Sex.Value;
            if (!string.IsNullOrEmpty(request.Email)) patientToUpdate.Email = request.Email;
            if (!string.IsNullOrEmpty(request.Phone)) patientToUpdate.Phone = request.Phone;
            if (request.Nationality != null) patientToUpdate.Nationality = request.Nationality;


            //Actualizar datos opcionales

           
            //Motivo de consulta
            if (!string.IsNullOrEmpty(request.PrincipalMotive)) patientToUpdate.PrincipalMotive = request.PrincipalMotive;
            if (!string.IsNullOrEmpty(request.ActualSymptoms)) patientToUpdate.ActualSymptoms = request.ActualSymptoms;
            if (!string.IsNullOrEmpty(request.RecentEvents)) patientToUpdate.RecentEvents = request.RecentEvents;
            if (!string.IsNullOrEmpty(request.PreviousDiagnosis)) patientToUpdate.PreviousDiagnosis = request.PreviousDiagnosis;
            
            //Historia clínica
            if (!string.IsNullOrEmpty(request.ProfesionalObservations)) patientToUpdate.ProfesionalObservations = request.ProfesionalObservations;
            if (!string.IsNullOrEmpty(request.KeyWords)) patientToUpdate.KeyWords = request.KeyWords;
            if (!string.IsNullOrEmpty(request.FailedActs)) patientToUpdate.FailedActs = request.FailedActs;
            if (!string.IsNullOrEmpty(request.Interconsulation)) patientToUpdate.Interconsulation = request.Interconsulation;
            if (!string.IsNullOrEmpty(request.PatientEvolution)) patientToUpdate.PatientEvolution = request.PatientEvolution;

            //Organización y seguimiento
            if (request.SessionDay.HasValue) patientToUpdate.SessionDay = request.SessionDay.Value;
            if (request.Modality.HasValue) patientToUpdate.Modality = request.Modality.Value;
            if (request.SessionDuration.HasValue) patientToUpdate.SessionDuration = request.SessionDuration.Value;
            if (!string.IsNullOrEmpty(request.SessionFrequency)) patientToUpdate.SessionFrequency = request.SessionFrequency;
            if (!string.IsNullOrEmpty(request.PreferedContact)) patientToUpdate.PreferedContact = request.PreferedContact;

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
