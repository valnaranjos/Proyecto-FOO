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

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }

        public DbSet<PatientMaterial> PatientMaterials { get; set; }

        public DbSet<PatientNote> PatientNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.Property(e => e.Email)
                      .HasColumnType("varchar(100)")
                      .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<VerificationCode>()
            .HasKey(vc => new { vc.UserId, vc.Code, vc.Purpose });

            modelBuilder.Entity<VerificationCode>()
            .HasIndex(vc => vc.Expiry);

            modelBuilder.Entity<PatientMaterial>()
            .HasOne(p => p.Patient)
            .WithMany(m => m.Materials)
            .HasForeignKey(p => p.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PatientNote>()
            .HasOne(p => p.Patient)
            .WithMany(n => n.Notes)
            .HasForeignKey(p => p.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }


    }
}
