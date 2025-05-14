using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProyectoFoo.Infrastructure.Context;
using System.IO;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Reflection;

namespace ProyectoFoo.Infrastructure.Factories
{
    public class ApplicationContextSqlServerFactory : IDesignTimeDbContextFactory<ApplicationContextSqlServer>
    {
        public ApplicationContextSqlServer CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var apiProjectPath = Path.Combine(basePath, "..", "ProyectoFoo.API"); // Ajusta la ruta relativa a tu proyecto de API

            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

            var builder = new DbContextOptionsBuilder<ApplicationContextSqlServer>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

            builder.UseMySql(connectionString, serverVersion);
            Console.WriteLine($"Cadena de Conexión (DesignTime): {connectionString}");

            return new ApplicationContextSqlServer(builder.Options);
        }
    }
}
