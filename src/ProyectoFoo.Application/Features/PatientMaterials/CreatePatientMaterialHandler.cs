using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain;
using ProyectoFoo.Shared;
using ProyectoFoo.Shared.Models;
using System;
using ProyectoFoo.Application.Features.Patients.CRUD;
using ProyectoFoo.Domain.Entities;

namespace ProyectoFoo.Application.Features.PatientMaterials
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
            if (patient == null)
            {
                return new CreatePatientMaterialResponse
                {
                    Success = false,
                    Message = $"No se encontró el paciente con ID: {request.PatientId}"
                };
            }

            // Validar la fecha de la sesión (no anterior a la actual)
            if (request.Material.Date.Date < DateTime.UtcNow.Date)
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
                Title = request.Material.Title,
                Date = request.Material.Date,
                Content = request.Material.Content,
                CreationDate = DateTime.UtcNow // Establecer la fecha de creación
            };

            try
            {
                var newPacienteMaterialEntity = await _patientMaterialRepository.AddAsync(patientMaterialEntity);
                var patientMaterialDto = new PatientMaterialDto
                {
                    Id = request.PatientId,
                    Title = request.Material.Title,
                    Date = request.Material.Date,
                    Content = request.Material.Content
                };

                return new CreatePatientMaterialResponse
                {
                    PatientMaterial = patientMaterialDto,
                    Success = true,
                    Message = "Material para el paciente creado exitosamente."
                };
            }
            catch (Exception ex)
            {
                return new CreatePatientMaterialResponse
                {
                    Success = false,
                    Message = "Hubo un error al crear el material del paciente: " + ex.Message
                };
            }
        }
    }
}
