using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFoo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameApellidoToSurnameAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
               name: "Apellido",
               table: "Usuarios",
               newName: "Surname");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
               name: "Surname",
               table: "Usuarios",
               newName: "Apellido");

        }
    }
}
