using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;


namespace ProyectoFoo.Application.ServiceExtensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            // Agg otras configuraciones específicas de la CAPA APPLICATION
            return services;
        }
    }
}
