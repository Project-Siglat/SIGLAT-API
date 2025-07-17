using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class relatedroleidentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Identity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_Role",
                table: "Identity",
                column: "Role");

            migrationBuilder.AddForeignKey(
                name: "FK_Identity_Roles_Role",
                table: "Identity",
                column: "Role",
                principalTable: "Roles",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identity_Roles_Role",
                table: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Identity_Role",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Identity");
        }
    }
}
