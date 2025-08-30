using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyUserLoginTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInfo",
                table: "UserLoginTracking");

            migrationBuilder.DropColumn(
                name: "Browser",
                table: "UserLoginTracking");

            migrationBuilder.DropColumn(
                name: "City",
                table: "UserLoginTracking");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "UserLoginTracking");

            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "UserLoginTracking");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "UserLoginTracking");

            migrationBuilder.DropColumn(
                name: "OperatingSystem",
                table: "UserLoginTracking");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "UserLoginTracking");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "UserLoginTracking");

            migrationBuilder.AlterColumn<string>(
                name: "FailureReason",
                table: "UserLoginTracking",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FailureReason",
                table: "UserLoginTracking",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInfo",
                table: "UserLoginTracking",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Browser",
                table: "UserLoginTracking",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "UserLoginTracking",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "UserLoginTracking",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceType",
                table: "UserLoginTracking",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "UserLoginTracking",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OperatingSystem",
                table: "UserLoginTracking",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "UserLoginTracking",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "UserLoginTracking",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
