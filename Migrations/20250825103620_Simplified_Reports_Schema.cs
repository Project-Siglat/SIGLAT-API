using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class Simplified_Reports_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Reports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Reports",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Reports",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Reports",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Reports",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
