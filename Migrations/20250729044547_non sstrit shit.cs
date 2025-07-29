using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class nonsstritshit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Identity_Responder",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Identity_Uid",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_Responder",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_Uid",
                table: "Alerts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Alerts_Responder",
                table: "Alerts",
                column: "Responder");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_Uid",
                table: "Alerts",
                column: "Uid");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Identity_Responder",
                table: "Alerts",
                column: "Responder",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Identity_Uid",
                table: "Alerts",
                column: "Uid",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
