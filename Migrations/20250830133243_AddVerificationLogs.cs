using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddVerificationLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VerificationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VerificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviousStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NewStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AdminRemarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ActionTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationLogs_Identity_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "Identity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VerificationLogs_Verifications_VerificationId",
                        column: x => x.VerificationId,
                        principalTable: "Verifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 32, 42, 325, DateTimeKind.Utc).AddTicks(212), new DateTime(2025, 8, 30, 13, 32, 42, 325, DateTimeKind.Utc).AddTicks(214) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 32, 42, 325, DateTimeKind.Utc).AddTicks(220), new DateTime(2025, 8, 30, 13, 32, 42, 325, DateTimeKind.Utc).AddTicks(220) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 32, 42, 325, DateTimeKind.Utc).AddTicks(221), new DateTime(2025, 8, 30, 13, 32, 42, 325, DateTimeKind.Utc).AddTicks(222) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 32, 42, 325, DateTimeKind.Utc).AddTicks(223), new DateTime(2025, 8, 30, 13, 32, 42, 325, DateTimeKind.Utc).AddTicks(223) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 32, 42, 325, DateTimeKind.Utc).AddTicks(224), new DateTime(2025, 8, 30, 13, 32, 42, 325, DateTimeKind.Utc).AddTicks(225) });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationLogs_AdminUserId",
                table: "VerificationLogs",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationLogs_VerificationId",
                table: "VerificationLogs",
                column: "VerificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerificationLogs");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 8, 42, 184, DateTimeKind.Utc).AddTicks(5196), new DateTime(2025, 8, 30, 13, 8, 42, 184, DateTimeKind.Utc).AddTicks(5198) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 8, 42, 184, DateTimeKind.Utc).AddTicks(5200), new DateTime(2025, 8, 30, 13, 8, 42, 184, DateTimeKind.Utc).AddTicks(5201) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 8, 42, 184, DateTimeKind.Utc).AddTicks(5202), new DateTime(2025, 8, 30, 13, 8, 42, 184, DateTimeKind.Utc).AddTicks(5202) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 8, 42, 184, DateTimeKind.Utc).AddTicks(5203), new DateTime(2025, 8, 30, 13, 8, 42, 184, DateTimeKind.Utc).AddTicks(5203) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 13, 8, 42, 184, DateTimeKind.Utc).AddTicks(5204), new DateTime(2025, 8, 30, 13, 8, 42, 184, DateTimeKind.Utc).AddTicks(5205) });
        }
    }
}
