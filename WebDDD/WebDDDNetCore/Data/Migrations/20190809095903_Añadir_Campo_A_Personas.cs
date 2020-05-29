using Microsoft.EntityFrameworkCore.Migrations;

namespace WebDDDNetCore.Data.Migrations
{
    public partial class Añadir_Campo_A_Personas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
               name: "Telefono",
               table: "Personas",
               nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "Telefono",
               table: "Personas");
        }
    }
}
