using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class MakeDescriptionNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Contact",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 31, 5, 43, 35, 188, DateTimeKind.Utc).AddTicks(3815), new DateTime(2025, 8, 31, 5, 43, 35, 188, DateTimeKind.Utc).AddTicks(3815) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 31, 5, 43, 35, 188, DateTimeKind.Utc).AddTicks(3819), new DateTime(2025, 8, 31, 5, 43, 35, 188, DateTimeKind.Utc).AddTicks(3819) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 31, 5, 43, 35, 188, DateTimeKind.Utc).AddTicks(3821), new DateTime(2025, 8, 31, 5, 43, 35, 188, DateTimeKind.Utc).AddTicks(3821) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 31, 5, 43, 35, 188, DateTimeKind.Utc).AddTicks(3823), new DateTime(2025, 8, 31, 5, 43, 35, 188, DateTimeKind.Utc).AddTicks(3823) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 31, 5, 43, 35, 188, DateTimeKind.Utc).AddTicks(3824), new DateTime(2025, 8, 31, 5, 43, 35, 188, DateTimeKind.Utc).AddTicks(3824) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Contact",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 31, 5, 42, 36, 505, DateTimeKind.Utc).AddTicks(1208), new DateTime(2025, 8, 31, 5, 42, 36, 505, DateTimeKind.Utc).AddTicks(1209) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 31, 5, 42, 36, 505, DateTimeKind.Utc).AddTicks(1211), new DateTime(2025, 8, 31, 5, 42, 36, 505, DateTimeKind.Utc).AddTicks(1211) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 31, 5, 42, 36, 505, DateTimeKind.Utc).AddTicks(1212), new DateTime(2025, 8, 31, 5, 42, 36, 505, DateTimeKind.Utc).AddTicks(1212) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 31, 5, 42, 36, 505, DateTimeKind.Utc).AddTicks(1213), new DateTime(2025, 8, 31, 5, 42, 36, 505, DateTimeKind.Utc).AddTicks(1214) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 31, 5, 42, 36, 505, DateTimeKind.Utc).AddTicks(1214), new DateTime(2025, 8, 31, 5, 42, 36, 505, DateTimeKind.Utc).AddTicks(1215) });
        }
    }
}
