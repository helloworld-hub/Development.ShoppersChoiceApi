using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppersChoice.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangedProductModel1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "thumbnals",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "thumbnails",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "thumbnails",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "thumbnals",
                table: "Products",
                type: "int",
                nullable: true);
        }
    }
}
