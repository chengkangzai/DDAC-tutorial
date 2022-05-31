using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NewNewTry.Migrations.NewNewTryNew
{
    public partial class CreateFlowerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flower",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlowerName = table.Column<string>(nullable: true),
                    FlowerPrice = table.Column<decimal>(nullable: false),
                    FlowerType = table.Column<string>(nullable: true),
                    FlowerSentToShop = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flower", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flower");
        }
    }
}
