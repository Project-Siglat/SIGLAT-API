using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class RefactorUserLoginTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identity_Roles_RoleId",
                table: "Identity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLoginTracking_Identity_UserId",
                table: "UserLoginTracking");

            migrationBuilder.DropIndex(
                name: "IX_UserLoginTracking_UserId",
                table: "UserLoginTracking");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LogoutTimestamp",
                table: "UserLoginTracking",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                table: "Identity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Identity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Identity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Identity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 18, 56, 13, 378, DateTimeKind.Utc).AddTicks(6409), new DateTime(2025, 8, 30, 18, 56, 13, 378, DateTimeKind.Utc).AddTicks(6409) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 18, 56, 13, 378, DateTimeKind.Utc).AddTicks(6411), new DateTime(2025, 8, 30, 18, 56, 13, 378, DateTimeKind.Utc).AddTicks(6411) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 18, 56, 13, 378, DateTimeKind.Utc).AddTicks(6412), new DateTime(2025, 8, 30, 18, 56, 13, 378, DateTimeKind.Utc).AddTicks(6413) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 18, 56, 13, 378, DateTimeKind.Utc).AddTicks(6413), new DateTime(2025, 8, 30, 18, 56, 13, 378, DateTimeKind.Utc).AddTicks(6414) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 18, 56, 13, 378, DateTimeKind.Utc).AddTicks(6414), new DateTime(2025, 8, 30, 18, 56, 13, 378, DateTimeKind.Utc).AddTicks(6415) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LogoutTimestamp",
                table: "UserLoginTracking",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                table: "Identity",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Identity",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Identity",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Identity",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 16, 5, 12, 650, DateTimeKind.Utc).AddTicks(7358), new DateTime(2025, 8, 30, 16, 5, 12, 650, DateTimeKind.Utc).AddTicks(7359) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 16, 5, 12, 650, DateTimeKind.Utc).AddTicks(7363), new DateTime(2025, 8, 30, 16, 5, 12, 650, DateTimeKind.Utc).AddTicks(7363) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 16, 5, 12, 650, DateTimeKind.Utc).AddTicks(7364), new DateTime(2025, 8, 30, 16, 5, 12, 650, DateTimeKind.Utc).AddTicks(7365) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 16, 5, 12, 650, DateTimeKind.Utc).AddTicks(7366), new DateTime(2025, 8, 30, 16, 5, 12, 650, DateTimeKind.Utc).AddTicks(7366) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 16, 5, 12, 650, DateTimeKind.Utc).AddTicks(7367), new DateTime(2025, 8, 30, 16, 5, 12, 650, DateTimeKind.Utc).AddTicks(7368) });

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginTracking_UserId",
                table: "UserLoginTracking",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Identity_Roles_RoleId",
                table: "Identity",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLoginTracking_Identity_UserId",
                table: "UserLoginTracking",
                column: "UserId",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
