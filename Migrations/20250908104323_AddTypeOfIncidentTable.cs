using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeOfIncidentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TypeOfIncidents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameOfIncident = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AddedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WhoAddedItID = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeOfIncidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TypeOfIncidents_Identity_WhoAddedItID",
                        column: x => x.WhoAddedItID,
                        principalTable: "Identity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_TypeOfIncidents_WhoAddedItID",
                table: "TypeOfIncidents",
                column: "WhoAddedItID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TypeOfIncidents");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1386), new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1387) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1390), new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1390) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1391), new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1391) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1392), new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1393) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1394), new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1394) });

            migrationBuilder.UpdateData(
                table: "VerificationTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1578), new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1579) });

            migrationBuilder.UpdateData(
                table: "VerificationTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1581), new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1582) });

            migrationBuilder.UpdateData(
                table: "VerificationTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1583), new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1583) });
        }
    }
}
