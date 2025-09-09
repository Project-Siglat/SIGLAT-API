using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddBFPAndPNPColumnsToTypeOfIncident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isBFPTrue",
                table: "TypeOfIncidents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isPNPTrue",
                table: "TypeOfIncidents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(5858), new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(5859) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(5864), new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(5864) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(5866), new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(5867) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(5868), new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(5868) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(5870), new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(5871) });

            migrationBuilder.UpdateData(
                table: "VerificationTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(6073), new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(6074) });

            migrationBuilder.UpdateData(
                table: "VerificationTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(6078), new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(6079) });

            migrationBuilder.UpdateData(
                table: "VerificationTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(6081), new DateTime(2025, 9, 8, 11, 6, 5, 116, DateTimeKind.Utc).AddTicks(6081) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isBFPTrue",
                table: "TypeOfIncidents");

            migrationBuilder.DropColumn(
                name: "isPNPTrue",
                table: "TypeOfIncidents");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6200), new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6201) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6204), new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6205) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6206), new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6206) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6207), new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6207) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6208), new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6208) });

            migrationBuilder.UpdateData(
                table: "VerificationTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6284), new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6284) });

            migrationBuilder.UpdateData(
                table: "VerificationTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6286), new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6287) });

            migrationBuilder.UpdateData(
                table: "VerificationTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6288), new DateTime(2025, 9, 8, 10, 43, 21, 795, DateTimeKind.Utc).AddTicks(6288) });
        }
    }
}
