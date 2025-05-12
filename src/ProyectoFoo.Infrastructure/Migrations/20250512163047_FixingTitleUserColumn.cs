using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFoo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixingTitleUserColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
       name: "Title",
       table: "Usuarios",
       type: "varchar(100)",
       maxLength: 100,
       nullable: true)
       .Annotation("MySql:CharSet", "utf8mb4");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
        name: "Title",
        table: "Usuarios");
        }
    }
}
