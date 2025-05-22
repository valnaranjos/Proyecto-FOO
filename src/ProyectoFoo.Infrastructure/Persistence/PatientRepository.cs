using MediatR;
using Microsoft.EntityFrameworkCore;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Domain.Common.Enums;
using ProyectoFoo.Domain.Entities;
using ProyectoFoo.Infrastructure.Context;
using System.Linq.Expressions;

namespace ProyectoFoo.Infrastructure.Persistence
{
    public class PatientRepository : BaseRepository<Paciente>, IPatientRepository
    {

        //Como hereda de BaseRepository, no es necesario implementar los métodos dque hereda, los demás sí.

        private new readonly ApplicationContextSqlServer _dbContext;


        public PatientRepository(ApplicationContextSqlServer dbContext) : base(dbContext) => _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

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
            var pacientes = await  _dbContext.Pacientes
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
                                 .Where(p => p.UserId == userId) // FILTRADO CRÍTICO
                                 .ToListAsync();
        }

        public async Task<List<Paciente>> GetByModalityAsync(string modality)
        {
            // Intentamos convertir el string a un valor del enum ModalityType
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
