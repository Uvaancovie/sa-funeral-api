using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAFuneralSuppliesAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddColorVariations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorVariations",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 2, 17, 20, 24, 39, 858, DateTimeKind.Utc).AddTicks(2456), "$2a$11$9tRGj25cMpeaDduluEt5uunA1XZp68xRmRDpALQJj.ghFJKVobSDe" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorVariations",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 2, 17, 19, 19, 40, 135, DateTimeKind.Utc).AddTicks(1586), "$2a$11$bGd9g/AC30aNC8O0Rg7fsuOxCZ/zoT4JDt1M/VwHWDj/K4J0fzcqG" });
        }
    }
}
