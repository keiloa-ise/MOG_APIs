using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleChangeLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRoleChangeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PreviousRoleId = table.Column<int>(type: "int", nullable: false),
                    NewRoleId = table.Column<int>(type: "int", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleChangeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoleChangeLogs_AppUsers_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoleChangeLogs_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoleChangeLogs_Roles_NewRoleId",
                        column: x => x.NewRoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoleChangeLogs_Roles_PreviousRoleId",
                        column: x => x.PreviousRoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 9, 13, 20, 633, DateTimeKind.Utc).AddTicks(1334));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 9, 13, 20, 633, DateTimeKind.Utc).AddTicks(1509));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 9, 13, 20, 633, DateTimeKind.Utc).AddTicks(1510));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 9, 13, 20, 633, DateTimeKind.Utc).AddTicks(1510));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 9, 13, 20, 633, DateTimeKind.Utc).AddTicks(1511));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 9, 13, 20, 633, DateTimeKind.Utc).AddTicks(1512));

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleChangeLogs_ChangedByUserId",
                table: "UserRoleChangeLogs",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleChangeLogs_CreatedAt",
                table: "UserRoleChangeLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleChangeLogs_NewRoleId",
                table: "UserRoleChangeLogs",
                column: "NewRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleChangeLogs_PreviousRoleId",
                table: "UserRoleChangeLogs",
                column: "PreviousRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleChangeLogs_UserId",
                table: "UserRoleChangeLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoleChangeLogs");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 8, 14, 30, 878, DateTimeKind.Utc).AddTicks(1650));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 8, 14, 30, 878, DateTimeKind.Utc).AddTicks(2124));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 8, 14, 30, 878, DateTimeKind.Utc).AddTicks(2126));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 8, 14, 30, 878, DateTimeKind.Utc).AddTicks(2127));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 8, 14, 30, 878, DateTimeKind.Utc).AddTicks(2128));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 8, 14, 30, 878, DateTimeKind.Utc).AddTicks(2129));
        }
    }
}
