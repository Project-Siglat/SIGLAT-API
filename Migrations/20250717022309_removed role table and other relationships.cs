using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class removedroletableandotherrelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identity_Roles_Role",
                table: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Identity_Role",
                table: "Identity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "RoleDto");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleDto",
                table: "RoleDto",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleDto",
                table: "RoleDto");

            migrationBuilder.RenameTable(
                name: "RoleDto",
                newName: "Roles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Name");

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
    }
}
