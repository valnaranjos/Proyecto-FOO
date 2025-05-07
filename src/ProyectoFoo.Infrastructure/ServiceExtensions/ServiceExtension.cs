using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Infrastructure.Persistence;

namespace ProyectoFoo.Infrastructure.ServiceExtensions
{
    public static class ServiceExtension    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UsuarioRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            // Registra aquí otros servicios de infraestructura (repositorios, etc.)
            return services;
        } 
    }
}
