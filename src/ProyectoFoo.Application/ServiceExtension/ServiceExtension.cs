﻿using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.ServiceExtension;
using ProyectoFoo.Domain.Entities;


namespace ProyectoFoo.Application.ServiceExtension
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddSingleton<IVerificationCodeService, VerificationCodeService>();
            services.AddScoped<IVerificationFlowService, VerificationFlowService>();
            return services;
        }
    }
}
