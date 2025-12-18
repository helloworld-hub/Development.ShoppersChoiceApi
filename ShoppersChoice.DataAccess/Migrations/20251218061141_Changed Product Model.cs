using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppersChoice.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangedProductModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "title",
                table: "Products",
                newName: "name");

            migrationBuilder.AddColumn<int>(
                name: "thumbnals",
                table: "Products",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "thumbnals",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Products",
                newName: "title");
        }
    }
}
