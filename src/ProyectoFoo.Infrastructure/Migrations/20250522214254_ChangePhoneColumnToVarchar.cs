using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFoo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangePhoneColumnToVarchar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
       name: "Phone",
       table: "Usuarios",
       type: "varchar(20)", 
       nullable: true,
       oldClrType: typeof(long),
       oldType: "bigint",
       oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
        name: "Phone",
        table: "Usuarios",
        type: "bigint",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "varchar(20)",
        oldNullable: true);
        }
    }
}
