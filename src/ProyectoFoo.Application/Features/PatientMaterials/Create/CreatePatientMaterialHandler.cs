using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain;
using ProyectoFoo.Shared;
using ProyectoFoo.Shared.Models;
using System;
using ProyectoFoo.Application.Features.Patients.CRUD;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Shared.Models.PatientMaterial;

namespace ProyectoFoo.Application.Features.PatientMaterials.Create
{
    public class CreatePatientMaterialHandler : IRequestHandler<CreatePatientMaterialCommand, CreatePatientMaterialResponse>
    {
        private readonly IPatientMaterialRepository _patientMaterialRepository;
        private readonly IPatientRepository _patientRepository;

        public CreatePatientMaterialHandler(IPatientMaterialRepository patientMaterialRepository, IPatientRepository patientRepository)
        {
            _patientMaterialRepository = patientMaterialRepository ?? throw new ArgumentNullException(nameof(patientMaterialRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        }

        public async Task<CreatePatientMaterialResponse> Handle(CreatePatientMaterialCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetByIdAsync(request.PatientId);

            try
            {
                if (patient == null)
                {
                    return new CreatePatientMaterialResponse
                    {
                        Success = false,
                        Message = $"No se encontró el paciente con ID: {request.PatientId}"
                    };
                }

                // Validar la fecha de la sesión (no anterior a la actual)
                if (request.Date < DateTime.UtcNow.Date)
                {
                    return new CreatePatientMaterialResponse
                    {
                        Success = false,
                        Message = "La fecha de la sesión no puede ser anterior a la fecha actual."
                    };
                }

                var patientMaterialEntity = new PatientMaterial
                {
                    PatientId = request.PatientId,
                    Title = request.Title,
                    Date = request.Date,
                    Content = request.Content,
                    CreationDate = DateTime.UtcNow
                };

                var newPacienteMaterialEntity = await _patientMaterialRepository.AddAsync(patientMaterialEntity);
                if (newPacienteMaterialEntity == null)
                {
                    return new CreatePatientMaterialResponse
                    {
                        Success = false,
                        Message = "No se pudo crear el material del paciente."
                    };
                }
                else
                {
                    return new CreatePatientMaterialResponse
                    {
                        Success = true,
                        Message = "Material del paciente creado exitosamente.",
                        PatientMaterialId = newPacienteMaterialEntity.Id,
                        Material = new PatientMaterialDto
                        {
                            Id = newPacienteMaterialEntity.Id,
                            PatientId = newPacienteMaterialEntity.PatientId,
                            Title = newPacienteMaterialEntity.Title,
                            Content = newPacienteMaterialEntity.Content,
                            CreationDate = newPacienteMaterialEntity.CreationDate
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                return new CreatePatientMaterialResponse
                {
                    Success = false,
                    Message = "Hubo un error inesperado al crear el material del paciente: " + ex.Message
                };
            }
        }
    }
}
