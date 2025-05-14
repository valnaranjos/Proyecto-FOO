using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFoo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewFieldsPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Modality",
                table: "Pacientes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ActualSymptoms",
                table: "Pacientes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FailedActs",
                table: "Pacientes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Interconsulation",
                table: "Pacientes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "KeyWords",
                table: "Pacientes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PatientEvolution",
                table: "Pacientes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PreferedContact",
                table: "Pacientes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PreviousDiagnosis",
                table: "Pacientes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PrincipalMotive",
                table: "Pacientes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProfesionalObservations",
                table: "Pacientes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "RecentEvents",
                table: "Pacientes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "SessionDay",
                table: "Pacientes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SessionDuration",
                table: "Pacientes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionFrequency",
                table: "Pacientes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualSymptoms",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "FailedActs",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "Interconsulation",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "KeyWords",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "PatientEvolution",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "PreferedContact",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "PreviousDiagnosis",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "PrincipalMotive",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "ProfesionalObservations",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "RecentEvents",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "SessionDay",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "SessionDuration",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "SessionFrequency",
                table: "Pacientes");

            migrationBuilder.AlterColumn<int>(
                name: "Modality",
                table: "Pacientes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
