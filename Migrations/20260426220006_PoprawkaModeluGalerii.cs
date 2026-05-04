using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarDealershipManager.Migrations
{
    /// <inheritdoc />
    public partial class PoprawkaModeluGalerii : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "Galleries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Galleries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Galleries");
        }
    }
}
