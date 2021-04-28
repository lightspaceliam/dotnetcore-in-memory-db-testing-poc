using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations
{
    public partial class UpdateNameProductJuncture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Data migration strategy required. We don't want to lose our ProductJuncture data.

            migrationBuilder.DropTable(
                name: "ProductJuncs");

            migrationBuilder.CreateTable(
                name: "ProductJunctures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    Price = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Juncture = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductJunctures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductJunctures_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "Index_ProductJuncture_ProductId_Juncture",
                table: "ProductJunctures",
                columns: new[] { "ProductId", "Juncture" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data migration strategy required. We don't want to lose our ProductJuncture data.

            migrationBuilder.DropTable(
                name: "ProductJunctures");

            migrationBuilder.CreateTable(
                name: "ProductJuncs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Juncture = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductJuncs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductJuncs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "Index_ProductJuncture_ProductId_Juncture",
                table: "ProductJuncs",
                columns: new[] { "ProductId", "Juncture" });
        }
    }
}
