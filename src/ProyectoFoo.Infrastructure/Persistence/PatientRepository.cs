using MediatR;
using Microsoft.EntityFrameworkCore;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Common.Enums;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Infrastructure.Context;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProyectoFoo.Infrastructure.Persistence
{
    public class PatientRepository(ApplicationContextSqlServer dbContext) : BaseRepository<Paciente>(dbContext), IPatientRepository
    {

        //Como hereda de BaseRepository, no es necesario implementar los métodos dque hereda, los demás sí.

        private new readonly ApplicationContextSqlServer _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<List<Paciente>> SearchAsync(
        string? fullName,
        string? identification,
        string? email,
        string? nationality,
        SexType? sexType,
        ModalityType? modality,
        string? ageRange
  )
        {
            IQueryable<Paciente> query = _dbContext.Pacientes;

            if (!string.IsNullOrWhiteSpace(fullName))
                query = query.Where(p =>
                    (p.Name + " " + p.Surname).Contains(fullName, StringComparison.CurrentCultureIgnoreCase));
            if (!string.IsNullOrWhiteSpace(identification))
                query = query.Where(p => p.Identification.Contains(identification));
            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(p => p.Email.Contains(email));
            if (!string.IsNullOrWhiteSpace(nationality))
                query = query.Where(p => p.Nationality.Contains(nationality));
            if (sexType.HasValue)
                query = query.Where(p => p.Sex == sexType.Value);
            if (modality.HasValue)
                query = query.Where(p => p.Modality == modality.Value);

            if (!string.IsNullOrWhiteSpace(ageRange))
                query = query.Where(p => p.AgeRange == ageRange);

            return await query.ToListAsync();
        }

        public async Task<Paciente?> GetByEmailAsync(string email)
        {
            var emailLower = email.ToLower();
            var paciente = await _dbContext.Pacientes
                .FirstOrDefaultAsync(p => p.Email != null && p.Email.Equals(emailLower, StringComparison.CurrentCultureIgnoreCase));

            if (paciente is null)
            {
                return null;
            }

            return paciente;
        }

        public async Task<Paciente?> GetByIdentificationAsync(string identification)
        {
            return await _dbContext.Pacientes
               .FirstOrDefaultAsync(p => p.Identification == identification);
        }

        public async Task<List<Paciente>> GetByNationalityAsync(string nationality)
        {
            string nacionalidadBuscada = nationality.Trim();
            var pacientes = await _dbContext.Pacientes
                .Where(p => EF.Functions.Like(p.Nationality, nacionalidadBuscada))
                .ToListAsync();
            return pacientes;
        }

        public async Task<List<Paciente>> ListPatientsAsync(Expression<Func<Paciente, bool>> predicate)
        {
            return await _dbContext.Set<Paciente>().Where(predicate).ToListAsync();
        }

        public async Task<List<Paciente>> GetPatientsByUserIdAsync(int userId)
        {
            return await _dbContext.Pacientes
                                 .Where(p => p.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<Paciente?> GetByIdAndUserAsync(int patientId, int userId)
        {
            return await _dbContext.Pacientes
                            .FirstOrDefaultAsync(p => p.Id == patientId && p.UserId == userId);
        }
        public async Task<List<Paciente>> GetByModalityAsync(string modality)
        {
            if (!Enum.TryParse(modality, true, out ModalityType parsedModality))
            {
                throw new ArgumentException("La modalidad proporcionada no es válida.");
            }

            return await _dbContext.Pacientes
                .Where(p => p.Modality == parsedModality)
                .ToListAsync();
        }

        public async Task<List<Paciente>> GetBySexTypeAsync(SexType sex)
        {
            return await _dbContext.Pacientes
                 .Where(p => p.Sex == sex)
                 .ToListAsync();
        }

        public async Task<List<Paciente>> GetByAgeRangeAsync(string ageRange)
        {
            return await _dbContext.Pacientes
                .Where(p => p.AgeRange.Equals(ageRange, StringComparison.CurrentCultureIgnoreCase))
                .ToListAsync();
        }

    }
}
