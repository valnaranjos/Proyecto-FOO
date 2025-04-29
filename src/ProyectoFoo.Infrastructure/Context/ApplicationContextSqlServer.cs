using Microsoft.EntityFrameworkCore;
using ProyectoFoo.Shared.Models;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using ProyectoFoo.Domain.Entities;


namespace ProyectoFoo.Infrastructure.Context
{
    public class ApplicationContextSqlServer : DbContext
    {
        public ApplicationContextSqlServer(DbContextOptions<ApplicationContextSqlServer> options) 
            : base(options)
        {
        }

        //Entidades
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        //Modelo a crear
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        //Configuracion de la base de datos
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }


    }
}
