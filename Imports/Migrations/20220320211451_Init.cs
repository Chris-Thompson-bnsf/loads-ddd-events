using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imports.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoadImportedEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BolNumber = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadImportedEvent", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoadImportedEvent");
        }
    }
}
