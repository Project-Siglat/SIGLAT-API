using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class Rerolledtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Uid = table.Column<Guid>(type: "uuid", nullable: false),
                    Responder = table.Column<Guid>(type: "uuid", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Longitude = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Verifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UID = table.Column<Guid>(type: "uuid", nullable: false),
                    B64Image = table.Column<string>(type: "text", nullable: false),
                    VerificationType = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verifications", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "Verifications");
        }
    }
}
