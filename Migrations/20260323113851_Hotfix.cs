using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarDealershipManager.Migrations
{
    /// <inheritdoc />
    public partial class Hotfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProducentUntil",
                table: "Generations",
                newName: "ProducedUntil");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProducedUntil",
                table: "Generations",
                newName: "ProducentUntil");
        }
    }
}
