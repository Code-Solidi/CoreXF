using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Auditing.Persistence.Migrations
{
    public partial class InitAuditing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Auditing");

            migrationBuilder.CreateTable(
                name: "AuditTrails",
                schema: "Auditing",
                columns: table => new
                {
                    Who = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    What = table.Column<int>(type: "int", nullable: false),
                    When = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Identity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MadeBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrails", x => new { x.Who, x.When, x.What, x.Type, x.Identity });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrails",
                schema: "Auditing");
        }
    }
}
