using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityLearning.Migrations
{
    public partial class BrowserCounter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrowserCounter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FireFox = table.Column<int>(nullable: false),
                    Safari = table.Column<int>(nullable: false),
                    Edge = table.Column<int>(nullable: false),
                    IE = table.Column<int>(nullable: false),
                    Chorome = table.Column<int>(nullable: false),
                    Other = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrowserCounter", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrowserCounter");
        }
    }
}
