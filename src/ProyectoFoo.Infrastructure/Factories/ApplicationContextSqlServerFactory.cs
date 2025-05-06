using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProyectoFoo.Infrastructure.Context;
using System.IO;

namespace ProyectoFoo.Infrastructure.Factories
{
    public class ApplicationContextSqlServerFactory : IDesignTimeDbContextFactory<ApplicationContextSqlServer>
    {
        public ApplicationContextSqlServer CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "ProyectoFoo.API")) // Va hasta directorio de la API (donde está el appsetting.json)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationContextSqlServer>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                b => b.MigrationsAssembly("ProyectoFoo.Infrastructure"));

            return new ApplicationContextSqlServer(builder.Options);
        }
    }
}
