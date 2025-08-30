using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixUserLoginTrackingNullables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserAgent",
                table: "UserLoginTracking",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LogoutTimestamp",
                table: "UserLoginTracking",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "FailureReason",
                table: "UserLoginTracking",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "AttemptedEmail",
                table: "UserLoginTracking",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserAgent",
                table: "UserLoginTracking",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

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
                name: "FailureReason",
                table: "UserLoginTracking",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AttemptedEmail",
                table: "UserLoginTracking",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

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
    }
}
