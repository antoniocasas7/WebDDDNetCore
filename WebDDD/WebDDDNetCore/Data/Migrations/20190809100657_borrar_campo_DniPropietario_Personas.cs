using Microsoft.EntityFrameworkCore.Migrations;

namespace WebDDDNetCore.Data.Migrations
{
    public partial class borrar_campo_DniPropietario_Personas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
              name: "DniPropietario",
              table: "Personas");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
              name: "DniPropietario",
              table: "Personas");
        }
    }
}
