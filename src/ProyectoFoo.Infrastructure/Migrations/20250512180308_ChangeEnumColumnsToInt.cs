using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFoo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEnumColumnsToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Cambiar la columna 'Sex' de varchar(1) a int para coherencia con el enum.
            migrationBuilder.AlterColumn<int>(
                name: "Sex",
                table: "Pacientes",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1)");

            // Cambiar la columna 'Modality' a int para coherencia con el enum.
            migrationBuilder.AlterColumn<int>(
                name: "Modality",
                table: "Pacientes",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertir los cambios en caso de ser necesario.
            migrationBuilder.AlterColumn<string>(
                name: "Sex",
                table: "Pacientes",
                type: "varchar(1)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Modality",
                table: "Pacientes",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
