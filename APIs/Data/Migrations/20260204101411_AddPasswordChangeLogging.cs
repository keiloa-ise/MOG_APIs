using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordChangeLogging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordChangeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordChangeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordChangeLogs_AppUsers_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PasswordChangeLogs_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PasswordHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordHistories_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 10, 14, 9, 963, DateTimeKind.Utc).AddTicks(9018));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 10, 14, 9, 963, DateTimeKind.Utc).AddTicks(9176));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 10, 14, 9, 963, DateTimeKind.Utc).AddTicks(9177));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 10, 14, 9, 963, DateTimeKind.Utc).AddTicks(9178));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 10, 14, 9, 963, DateTimeKind.Utc).AddTicks(9179));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 4, 10, 14, 9, 963, DateTimeKind.Utc).AddTicks(9180));

            migrationBuilder.CreateIndex(
                name: "IX_PasswordChangeLogs_ChangedByUserId",
                table: "PasswordChangeLogs",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordChangeLogs_CreatedAt",
                table: "PasswordChangeLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordChangeLogs_UserId",
                table: "PasswordChangeLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordHistories_CreatedAt",
                table: "PasswordHistories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordHistories_UserId",
                table: "PasswordHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordChangeLogs");

            migrationBuilder.DropTable(
                name: "PasswordHistories");

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
        }
    }
}
