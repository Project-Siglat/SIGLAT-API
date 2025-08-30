using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGLATAPI.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeSchemaRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coordinates_Identity_DriverId",
                table: "Coordinates");

            migrationBuilder.DropForeignKey(
                name: "FK_Identity_Roles_RoleDtoId",
                table: "Identity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLoginTracking_Identity_IdentityId",
                table: "UserLoginTracking");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLoginTracking_Identity_UserId",
                table: "UserLoginTracking");

            migrationBuilder.DropForeignKey(
                name: "FK_Verifications_Identity_Id",
                table: "Verifications");

            migrationBuilder.DropTable(
                name: "LoginLogs");

            migrationBuilder.DropTable(
                name: "UserXYZ");

            migrationBuilder.DropIndex(
                name: "IX_UserLoginTracking_IdentityId",
                table: "UserLoginTracking");

            migrationBuilder.DropIndex(
                name: "IX_Identity_RoleDtoId",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "IdentityId",
                table: "UserLoginTracking");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "RoleDtoId",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "Responder",
                table: "Alerts");

            migrationBuilder.RenameColumn(
                name: "DriverId",
                table: "Coordinates",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Coordinates_DriverId",
                table: "Coordinates",
                newName: "IX_Coordinates_UserId");

            migrationBuilder.RenameColumn(
                name: "Sender",
                table: "Chat",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "Reciever",
                table: "Chat",
                newName: "ReceiverId");

            migrationBuilder.RenameColumn(
                name: "Uid",
                table: "Alerts",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "VerificationType",
                table: "Verifications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Verifications",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "Verifications",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Verifications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Identity",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Longitude",
                table: "Coordinates",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Latitude",
                table: "Coordinates",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Coordinates",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "Contact",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ContactType",
                table: "Contact",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ContactInformation",
                table: "Contact",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Contact",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Chat",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "What",
                table: "Alerts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Alerts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RespondedAt",
                table: "Alerts",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Longitude",
                table: "Alerts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Latitude",
                table: "Alerts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Alerts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ResponderId",
                table: "Alerts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Alerts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_UserId",
                table: "Verifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_RoleId",
                table: "Identity",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_UserId",
                table: "Contact",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_ReceiverId",
                table: "Chat",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_SenderId",
                table: "Chat",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_ResponderId",
                table: "Alerts",
                column: "ResponderId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_UserId",
                table: "Alerts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Identity_ResponderId",
                table: "Alerts",
                column: "ResponderId",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Identity_UserId",
                table: "Alerts",
                column: "UserId",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_Identity_ReceiverId",
                table: "Chat",
                column: "ReceiverId",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_Identity_SenderId",
                table: "Chat",
                column: "SenderId",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_Identity_UserId",
                table: "Contact",
                column: "UserId",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Coordinates_Identity_UserId",
                table: "Coordinates",
                column: "UserId",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Identity_Roles_RoleId",
                table: "Identity",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLoginTracking_Identity_UserId",
                table: "UserLoginTracking",
                column: "UserId",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Verifications_Identity_UserId",
                table: "Verifications",
                column: "UserId",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Identity_ResponderId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Identity_UserId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Chat_Identity_ReceiverId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_Chat_Identity_SenderId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_Identity_UserId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_Coordinates_Identity_UserId",
                table: "Coordinates");

            migrationBuilder.DropForeignKey(
                name: "FK_Identity_Roles_RoleId",
                table: "Identity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLoginTracking_Identity_UserId",
                table: "UserLoginTracking");

            migrationBuilder.DropForeignKey(
                name: "FK_Verifications_Identity_UserId",
                table: "Verifications");

            migrationBuilder.DropIndex(
                name: "IX_Verifications_UserId",
                table: "Verifications");

            migrationBuilder.DropIndex(
                name: "IX_Identity_RoleId",
                table: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Contact_UserId",
                table: "Contact");

            migrationBuilder.DropIndex(
                name: "IX_Chat_ReceiverId",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_SenderId",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_ResponderId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_UserId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Verifications");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Identity");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Coordinates");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "ResponderId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Alerts");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Coordinates",
                newName: "DriverId");

            migrationBuilder.RenameIndex(
                name: "IX_Coordinates_UserId",
                table: "Coordinates",
                newName: "IX_Coordinates_DriverId");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Chat",
                newName: "Sender");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Chat",
                newName: "Reciever");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Alerts",
                newName: "Uid");

            migrationBuilder.AlterColumn<string>(
                name: "VerificationType",
                table: "Verifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Verifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "Verifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<Guid>(
                name: "IdentityId",
                table: "UserLoginTracking",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Identity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RoleDtoId",
                table: "Identity",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Longitude",
                table: "Coordinates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Latitude",
                table: "Coordinates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "Contact",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ContactType",
                table: "Contact",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ContactInformation",
                table: "Contact",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Chat",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "What",
                table: "Alerts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Alerts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RespondedAt",
                table: "Alerts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Longitude",
                table: "Alerts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Latitude",
                table: "Alerts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<Guid>(
                name: "Responder",
                table: "Alerts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "LoginLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    What = table.Column<string>(type: "text", nullable: false),
                    Who = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserXYZ",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Latitude = table.Column<string>(type: "text", nullable: false),
                    Longitude = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserXYZ", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6176), new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6177) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6182), new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6183) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6184), new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6184) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6185), new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6186) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6187), new DateTime(2025, 8, 30, 12, 49, 17, 851, DateTimeKind.Utc).AddTicks(6187) });

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginTracking_IdentityId",
                table: "UserLoginTracking",
                column: "IdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_RoleDtoId",
                table: "Identity",
                column: "RoleDtoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coordinates_Identity_DriverId",
                table: "Coordinates",
                column: "DriverId",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Identity_Roles_RoleDtoId",
                table: "Identity",
                column: "RoleDtoId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLoginTracking_Identity_IdentityId",
                table: "UserLoginTracking",
                column: "IdentityId",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLoginTracking_Identity_UserId",
                table: "UserLoginTracking",
                column: "UserId",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Verifications_Identity_Id",
                table: "Verifications",
                column: "Id",
                principalTable: "Identity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
