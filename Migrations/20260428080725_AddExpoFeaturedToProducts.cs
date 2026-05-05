using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAFuneralSuppliesAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddExpoFeaturedToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExpoFeatured",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpoFeatured",
                table: "Products");
        }
    }
}
