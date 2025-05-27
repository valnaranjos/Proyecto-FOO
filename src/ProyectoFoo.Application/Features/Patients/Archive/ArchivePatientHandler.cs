using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Patients.Archive
{
    public class ArchivePatientHandler : IRequestHandler<ArchivePatientCommand, ArchivePatientResponse>
    {
        private readonly IPatientRepository _pacienteRepository;

        public ArchivePatientHandler(IPatientRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<ArchivePatientResponse> Handle(ArchivePatientCommand request, CancellationToken cancellationToken)
        {
            var patientToArchive = await _pacienteRepository.GetByIdAndUserAsync(request.Id, request.UserId);

            if (patientToArchive == null)
            {
                return new ArchivePatientResponse { Success = false, Message = $"No se encontró el paciente con el ID: {request.Id}" };
            }

            patientToArchive.IsEnabled = false; // Establece el estado a "archivado" (deshabilitado)

            try
            {
                await _pacienteRepository.UpdateAsync(patientToArchive);
                return new ArchivePatientResponse { Success = true, Message = $"Paciente con Id #{request.Id} fue archivado exitosamente." };
            }
            catch (Exception ex)
            {
                return new ArchivePatientResponse { Success = false, Message = $"Error al archivar el paciente: {ex.Message}" };
            }
        }
    }
}
