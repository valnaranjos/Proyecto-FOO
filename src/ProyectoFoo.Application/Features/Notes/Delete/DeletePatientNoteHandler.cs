using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Notes.Delete
{
public class DeletePatientNoteHandler : IRequestHandler<DeletePatientNoteCommand, DeletePatientNoteResponse>
{
    private readonly IPatientNoteRepository _patientNoteRepository;
    private readonly IPatientRepository _patientRepository;

        public DeletePatientNoteHandler(IPatientNoteRepository noteRepository, IPatientRepository patientRepository)
        {
            _patientNoteRepository = noteRepository ?? throw new ArgumentNullException(nameof(noteRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        }

        public async Task<DeletePatientNoteResponse> Handle(DeletePatientNoteCommand request, CancellationToken cancellationToken)
        {
            var response = new DeletePatientNoteResponse();
            var patient = await _patientRepository.GetByIdAsync(request.PatientId);
            if (patient == null)
            {
                response.Success = false;
                response.Message = $"No se encontró el paciente con ID: {request.PatientId}";
                return response;
            }

            var existingNote = await _patientNoteRepository.GetByIdAsync(request.NoteId);

            if (existingNote == null || existingNote.PatientId != request.PatientId)
            {
                response.Success = false;
                response.Message = $"No se encontró la nota con ID: {request.NoteId} para el paciente con ID: {request.PatientId}";
                return response;
            }

            try
            {
                await _patientNoteRepository.DeleteAsync(existingNote);

                return new DeletePatientNoteResponse
                {
                    Success = true,
                    Message = "Nota eliminada correctamente."
                };
            }
            catch (Exception ex)
            {
                return new DeletePatientNoteResponse
                {
                    Success = false,
                    Message = $"Error al eliminar la nota: {ex.Message}"
                };
            }
        }
}}
