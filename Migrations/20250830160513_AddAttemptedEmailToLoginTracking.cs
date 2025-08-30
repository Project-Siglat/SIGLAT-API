using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAttemptedEmailToLoginTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttemptedEmail",
                table: "UserLoginTracking",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttemptedEmail",
                table: "UserLoginTracking");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 43, 58, 518, DateTimeKind.Utc).AddTicks(6667), new DateTime(2025, 8, 30, 13, 43, 58, 518, DateTimeKind.Utc).AddTicks(6668) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 43, 58, 518, DateTimeKind.Utc).AddTicks(6671), new DateTime(2025, 8, 30, 13, 43, 58, 518, DateTimeKind.Utc).AddTicks(6671) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 43, 58, 518, DateTimeKind.Utc).AddTicks(6672), new DateTime(2025, 8, 30, 13, 43, 58, 518, DateTimeKind.Utc).AddTicks(6673) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 43, 58, 518, DateTimeKind.Utc).AddTicks(6674), new DateTime(2025, 8, 30, 13, 43, 58, 518, DateTimeKind.Utc).AddTicks(6675) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 43, 58, 518, DateTimeKind.Utc).AddTicks(6676), new DateTime(2025, 8, 30, 13, 43, 58, 518, DateTimeKind.Utc).AddTicks(6676) });
        }
    }
}
