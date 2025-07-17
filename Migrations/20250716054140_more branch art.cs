using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class morebranchart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Alerts_Responder",
                table: "Alerts",
                column: "Responder");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Identity_Responder",
                table: "Alerts",
                column: "Responder",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Identity_Responder",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_Responder",
                table: "Alerts");
        }
    }
}
