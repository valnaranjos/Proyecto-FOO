using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.PatientMaterials
{
    public class SendPatientMaterialEmailHandler(
        IPatientMaterialRepository patientMaterialRepository,
        IPatientRepository patientRepository,
        IEmailService emailService, IUserRepository userRepository) : IRequestHandler<SendPatientMaterialEmailCommand, SendPatientMaterialEmailResponse>
    {
        private readonly IPatientMaterialRepository _patientMaterialRepository = patientMaterialRepository ?? throw new ArgumentNullException(nameof(patientMaterialRepository));
        private readonly IPatientRepository _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        public async Task<SendPatientMaterialEmailResponse> Handle(SendPatientMaterialEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(request.PatientId);
                if (patient == null)
                {
                    return new SendPatientMaterialEmailResponse
                    {
                        Success = false,
                        Message = $"No se encontró el paciente con ID: {request.PatientId}."
                    };
                }

                if (string.IsNullOrWhiteSpace(patient.Email))
                {
                    return new SendPatientMaterialEmailResponse
                    {
                        Success = false,
                        Message = $"El paciente {patient.Name} {patient.Surname} (ID: {patient.Id}) no tiene una dirección de correo electrónico registrada."
                    };
                }
                
                var material = await _patientMaterialRepository.GetByIdAsync(request.MaterialId);
                
                if (material == null || material.PatientId != request.PatientId)
                {
                    return new SendPatientMaterialEmailResponse
                    {
                        Success = false,
                        Message = $"No se encontró el material con ID: {request.MaterialId} para el paciente con ID: {request.PatientId}."
                    };
                }

                var sendingUser = await _userRepository.GetByIdAsync(request.UserId);
                string senderName = "Insight"; 
                if (sendingUser != null)
                {                   
                    senderName = $"{sendingUser.Name} {sendingUser.Surname}";
                }

                var subject = $"Material de Sesión: {material.Title} - Insight";
                var body = $@"
                    <html>
                    <body>
                        <h2>Material para su sesión</h2>
                        <p>Estimado/a {patient.Name} {patient.Surname},</p>
                        <p>Le enviamos el material de su sesión con los siguientes detalles:</p>
                        <ul>
                            <li><strong>Título:</strong> {material.Title}</li>
                            <li><strong>Fecha de la Sesión:</strong> {material.Date.ToShortDateString()}</li>
                        </ul>
                        <p><strong>Contenido:</strong></p>
                        <div style='border: 1px solid #ccc; padding: 10px; margin-top: 15px; background-color: #f9f9f9; font-family: monospace;'>
                            {material.Content}
                        </div>
                        <p style='margin-top: 20px;'>Saludos cordiales,</p>
                         <p><strong>Atentamente, {senderName}</strong></p>
                        <p style='font-size: 0.8em; color: #888;'>Este es un mensaje generado automáticamente, por favor no lo responda.</p>
                    </body>
                    </html>";
             
                await _emailService.SendEmailAsync(patient.Email, subject, body);

                return new SendPatientMaterialEmailResponse
                {
                    Success = true,
                    Message = "Material enviado por correo electrónico exitosamente."
                };
            }
            catch (Exception ex)
            {              
                Console.WriteLine($"Error al intentar enviar el material por email: {ex.Message}");
               
                return new SendPatientMaterialEmailResponse
                {
                    Success = false,
                    Message = $"Hubo un error al intentar enviar el material por correo electrónico: {ex.Message}"
                };
            }
        }
    }
}
