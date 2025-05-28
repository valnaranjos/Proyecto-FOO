using ProyectoFoo.Domain.Common.Enums;
using ProyectoFoo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Contracts.Persistence
{
    /// <summary>
    /// Define el contrato para el repositorio de paciente.
    /// </summary>
    public interface IPatientRepository : IAsyncRepository<Paciente>
    {
        //Como hereda, no se deben implementar los métodos base.


        Task<List<Paciente>> ListPatientsAsync(Expression<Func<Paciente, bool>> predicate);
        Task<List<Paciente>> GetPatientsByUserIdAsync(int userId);
        Task<List<Paciente>> GetArchivedPatientsByUserIdAsync(int userId);
        Task<Paciente?> GetByIdAndUserAsync(int patientId, int userId);


        //BUSQUEDAS

        Task<List<Paciente>> SearchAsync(
        string? fullName,
        string? identification,
        string? email,
        string? nationality,
        SexType? sexType,
        ModalityType? modality,
        string? ageRange
        );
        Task<Paciente?> GetByEmailAsync(string email);

        Task<Paciente?> GetByIdentificationAsync(string identification);

        Task<List<Paciente>> GetByNationalityAsync(string nationality);   


        //FILTROS
        Task<List<Paciente>> GetByModalityAsync(string modality);
        Task<List<Paciente>> GetBySexTypeAsync(SexType sex);
        Task<List<Paciente>> GetByAgeRangeAsync(string ageRange);
    }
}
