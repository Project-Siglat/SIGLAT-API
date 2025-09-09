using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixDatabaseState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VerificationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    VerificationTypeId = table.Column<int>(type: "integer", nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DocumentName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DocumentImage = table.Column<byte[]>(type: "bytea", nullable: true),
                    ImageMimeType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AdminNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountVerifications_Identity_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Identity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AccountVerifications_Identity_UserId",
                        column: x => x.UserId,
                        principalTable: "Identity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountVerifications_VerificationTypes_VerificationTypeId",
                        column: x => x.VerificationTypeId,
                        principalTable: "VerificationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.InsertData(
                table: "VerificationTypes",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1578), "Philippine Passport", true, "Passport", new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1579) },
                    { 2, new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1581), "Philippine National ID (PhilID)", true, "National ID", new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1582) },
                    { 3, new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1583), "Valid Student ID from Educational Institution", true, "Student ID", new DateTime(2025, 9, 8, 5, 48, 17, 130, DateTimeKind.Utc).AddTicks(1583) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountVerifications_ReviewedByUserId",
                table: "AccountVerifications",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountVerifications_UserId",
                table: "AccountVerifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountVerifications_VerificationTypeId",
                table: "AccountVerifications",
                column: "VerificationTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountVerifications");

            migrationBuilder.DropTable(
                name: "VerificationTypes");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 6, 26, 58, 80, DateTimeKind.Utc).AddTicks(8467), new DateTime(2025, 9, 3, 6, 26, 58, 80, DateTimeKind.Utc).AddTicks(8469) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 6, 26, 58, 80, DateTimeKind.Utc).AddTicks(8472), new DateTime(2025, 9, 3, 6, 26, 58, 80, DateTimeKind.Utc).AddTicks(8473) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 6, 26, 58, 80, DateTimeKind.Utc).AddTicks(8474), new DateTime(2025, 9, 3, 6, 26, 58, 80, DateTimeKind.Utc).AddTicks(8474) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 6, 26, 58, 80, DateTimeKind.Utc).AddTicks(8475), new DateTime(2025, 9, 3, 6, 26, 58, 80, DateTimeKind.Utc).AddTicks(8475) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 3, 6, 26, 58, 80, DateTimeKind.Utc).AddTicks(8476), new DateTime(2025, 9, 3, 6, 26, 58, 80, DateTimeKind.Utc).AddTicks(8477) });
        }
    }
}
