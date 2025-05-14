using Microsoft.EntityFrameworkCore;
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
        public DbSet<VerificationCode> VerificationCodes { get; set; }

        public DbSet<PatientMaterial> PatientMaterials { get; set; }

        public DbSet<Note> Notes { get; set; }
        
        
        //Modelo a crear
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Aquí se puede mappear las entidades a la base de datos a gusto...

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.Property(e => e.Email)
                      .HasColumnType("varchar(100)")
                      .UseCollation("utf8_general_ci");  //Case-INSENSITIVE para busquedas por email.
            });

            modelBuilder.Entity<VerificationCode>()
            .HasKey(vc => new { vc.UserId, vc.Code, vc.Purpose }); // Clave primaria compuesta del codigo de verificación (necesaria, ya que no tiene ID como las otras entidades)

            modelBuilder.Entity<VerificationCode>()
            .HasIndex(vc => vc.Expiry);
        }

        //Configuracion de la base de datos
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }


    }
}
