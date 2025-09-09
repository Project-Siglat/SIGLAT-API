using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserLoginTrackingFieldTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 19, 4, 15, 404, DateTimeKind.Utc).AddTicks(1711), new DateTime(2025, 8, 30, 19, 4, 15, 404, DateTimeKind.Utc).AddTicks(1712) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 19, 4, 15, 404, DateTimeKind.Utc).AddTicks(1714), new DateTime(2025, 8, 30, 19, 4, 15, 404, DateTimeKind.Utc).AddTicks(1714) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 19, 4, 15, 404, DateTimeKind.Utc).AddTicks(1716), new DateTime(2025, 8, 30, 19, 4, 15, 404, DateTimeKind.Utc).AddTicks(1717) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 19, 4, 15, 404, DateTimeKind.Utc).AddTicks(1718), new DateTime(2025, 8, 30, 19, 4, 15, 404, DateTimeKind.Utc).AddTicks(1718) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 19, 4, 15, 404, DateTimeKind.Utc).AddTicks(1720), new DateTime(2025, 8, 30, 19, 4, 15, 404, DateTimeKind.Utc).AddTicks(1720) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 19, 2, 56, 723, DateTimeKind.Utc).AddTicks(962), new DateTime(2025, 8, 30, 19, 2, 56, 723, DateTimeKind.Utc).AddTicks(963) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 19, 2, 56, 723, DateTimeKind.Utc).AddTicks(964), new DateTime(2025, 8, 30, 19, 2, 56, 723, DateTimeKind.Utc).AddTicks(965) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 19, 2, 56, 723, DateTimeKind.Utc).AddTicks(966), new DateTime(2025, 8, 30, 19, 2, 56, 723, DateTimeKind.Utc).AddTicks(966) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 19, 2, 56, 723, DateTimeKind.Utc).AddTicks(967), new DateTime(2025, 8, 30, 19, 2, 56, 723, DateTimeKind.Utc).AddTicks(968) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 19, 2, 56, 723, DateTimeKind.Utc).AddTicks(969), new DateTime(2025, 8, 30, 19, 2, 56, 723, DateTimeKind.Utc).AddTicks(970) });
        }
    }
}
