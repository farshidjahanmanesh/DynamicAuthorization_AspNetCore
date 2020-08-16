using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityLearning.Migrations
{
    public partial class DeviceCounter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceCounter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IOS = table.Column<int>(nullable: false),
                    Android = table.Column<int>(nullable: false),
                    Desktop = table.Column<int>(nullable: false),
                    Other = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceCounter", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceCounter");
        }
    }
}
