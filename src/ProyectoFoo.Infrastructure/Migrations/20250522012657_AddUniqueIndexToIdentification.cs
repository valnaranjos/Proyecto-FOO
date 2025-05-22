using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFoo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToIdentification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
        name: "IX_Usuarios_Identification", // Nombre del índice
        table: "Usuarios", // Nombre de tu tabla de usuarios
        column: "Identification",
        unique: true // Esto lo hace único
    );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
        name: "IX_Usuarios_Identification",
        table: "Usuarios"
    );
        }
    }
}
