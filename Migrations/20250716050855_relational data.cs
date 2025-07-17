using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class relationaldata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Verifications_UID",
                table: "Verifications",
                column: "UID");

            migrationBuilder.CreateIndex(
                name: "IX_Coordinates_DriverId",
                table: "Coordinates",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_Uid",
                table: "Alerts",
                column: "Uid");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Identity_Uid",
                table: "Alerts",
                column: "Uid",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Coordinates_Identity_DriverId",
                table: "Coordinates",
                column: "DriverId",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Verifications_Identity_UID",
                table: "Verifications",
                column: "UID",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Identity_Uid",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Coordinates_Identity_DriverId",
                table: "Coordinates");

            migrationBuilder.DropForeignKey(
                name: "FK_Verifications_Identity_UID",
                table: "Verifications");

            migrationBuilder.DropIndex(
                name: "IX_Verifications_UID",
                table: "Verifications");

            migrationBuilder.DropIndex(
                name: "IX_Coordinates_DriverId",
                table: "Coordinates");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_Uid",
                table: "Alerts");
        }
    }
}
