using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarDealershipManager.Migrations
{
    /// <inheritdoc />
    public partial class hotfixcarstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_CarStatusModel_CarStatusId",
                table: "Cars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CarStatusModel",
                table: "CarStatusModel");

            migrationBuilder.RenameTable(
                name: "CarStatusModel",
                newName: "CarStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarStatus",
                table: "CarStatus",
                column: "CarStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_CarStatus_CarStatusId",
                table: "Cars",
                column: "CarStatusId",
                principalTable: "CarStatus",
                principalColumn: "CarStatusId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_CarStatus_CarStatusId",
                table: "Cars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CarStatus",
                table: "CarStatus");

            migrationBuilder.RenameTable(
                name: "CarStatus",
                newName: "CarStatusModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarStatusModel",
                table: "CarStatusModel",
                column: "CarStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_CarStatusModel_CarStatusId",
                table: "Cars",
                column: "CarStatusId",
                principalTable: "CarStatusModel",
                principalColumn: "CarStatusId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
