using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesTableWithSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleDtoId",
                table: "Identity",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6176), "System Administrator", "Admin", new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6177) },
                    { 2, new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6182), "Regular User", "User", new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6183) },
                    { 3, new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6184), "Ambulance Personnel", "Ambulance", new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6184) },
                    { 4, new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6185), "Philippine National Police", "PNP", new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6186) },
                    { 5, new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6187), "Bureau of Fire Protection", "BFP", new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6187) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Identity_RoleDtoId",
                table: "Identity",
                column: "RoleDtoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Identity_Roles_RoleDtoId",
                table: "Identity",
                column: "RoleDtoId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Identity_Roles_RoleDtoId",
                table: "Identity");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Identity_RoleDtoId",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "RoleDtoId",
                table: "Identity");
        }
    }
}
