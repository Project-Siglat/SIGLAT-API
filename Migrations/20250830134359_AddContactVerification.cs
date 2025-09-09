using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddContactVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerifiedAt",
                table: "Identity",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "Identity",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPhoneVerified",
                table: "Identity",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PhoneVerifiedAt",
                table: "Identity",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContactVerificationTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    VerificationType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ContactValue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    VerificationCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactVerificationTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactVerificationTokens_Identity_UserId",
                        column: x => x.UserId,
                        principalTable: "Identity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ContactVerificationTokens_UserId",
                table: "ContactVerificationTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactVerificationTokens");

            migrationBuilder.DropColumn(
                name: "EmailVerifiedAt",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "IsPhoneVerified",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "PhoneVerifiedAt",
                table: "Identity");

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
        }
    }
}
