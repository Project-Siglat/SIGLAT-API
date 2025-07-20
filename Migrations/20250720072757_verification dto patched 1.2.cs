using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class verificationdtopatched12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Verifications_Identity_UID",
                table: "Verifications");

            migrationBuilder.DropIndex(
                name: "IX_Verifications_UID",
                table: "Verifications");

            migrationBuilder.DropColumn(
                name: "UID",
                table: "Verifications");

            migrationBuilder.AddForeignKey(
                name: "FK_Verifications_Identity_Id",
                table: "Verifications",
                column: "Id",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Verifications_Identity_Id",
                table: "Verifications");

            migrationBuilder.AddColumn<Guid>(
                name: "UID",
                table: "Verifications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_UID",
                table: "Verifications",
                column: "UID");

            migrationBuilder.AddForeignKey(
                name: "FK_Verifications_Identity_UID",
                table: "Verifications",
                column: "UID",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
