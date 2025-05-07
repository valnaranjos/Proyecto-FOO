using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFoo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PatientAndUserUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "Institution",
                table: "Pacientes");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Pacientes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Pacientes");

            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "Pacientes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Institution",
                table: "Pacientes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
