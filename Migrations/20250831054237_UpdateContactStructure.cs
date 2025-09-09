using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContactStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Label",
                table: "Contact",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ContactType",
                table: "Contact",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "ContactInformation",
                table: "Contact",
                newName: "Value");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Contact",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Contact");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Contact",
                newName: "ContactInformation");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Contact",
                newName: "ContactType");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Contact",
                newName: "Label");

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
    }
}
