using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class TablePatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Identity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Identity",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Identity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Identity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Identity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Identity");
        }
    }
}
