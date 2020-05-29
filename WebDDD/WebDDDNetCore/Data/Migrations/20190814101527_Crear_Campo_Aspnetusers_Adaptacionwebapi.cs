using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace WebDDDNetCore.Data.Migrations
{
    public partial class Crear_Campo_Aspnetusers_Adaptacionwebapi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
               name: "LockoutEndDateUtc",
               table: "AspNetUsers",
               nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "LockoutEndDateUtc",
               table: "AspNetUsers");
        }
    }
}
