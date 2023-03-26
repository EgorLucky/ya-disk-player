using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    public partial class InitDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IgnorePaths",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: true),
                    PathHash = table.Column<string>(type: "text", nullable: true),
                    YandexUserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IgnorePaths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    YandexId = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Firstname = table.Column<string>(type: "text", nullable: true),
                    Lastname = table.Column<string>(type: "text", nullable: true),
                    Login = table.Column<string>(type: "text", nullable: true),
                    Sex = table.Column<string>(type: "text", nullable: true),
                    InviteId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActivateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeactivateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SynchronizationProcesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StartDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FinishedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Offset = table.Column<int>(type: "integer", nullable: false),
                    LastFileId = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynchronizationProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SynchronizationProcesses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: true),
                    ParentFolderPath = table.Column<string>(type: "text", nullable: true),
                    ParentFolder = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    YandexResourceId = table.Column<string>(type: "text", nullable: true),
                    YandexUserId = table.Column<string>(type: "text", nullable: true),
                    SynchronizationProcessId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_SynchronizationProcesses_SynchronizationProcessId",
                        column: x => x.SynchronizationProcessId,
                        principalTable: "SynchronizationProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SynchronizationProcessErrors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageText = table.Column<string>(type: "text", nullable: true),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SynchronizationProcessId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynchronizationProcessErrors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SynchronizationProcessErrors_SynchronizationProcesses_Synch~",
                        column: x => x.SynchronizationProcessId,
                        principalTable: "SynchronizationProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SynchronizationProcessUserCancellations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SynchronizationProcessId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynchronizationProcessUserCancellations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SynchronizationProcessUserCancellations_SynchronizationProc~",
                        column: x => x.SynchronizationProcessId,
                        principalTable: "SynchronizationProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_SynchronizationProcessId",
                table: "Files",
                column: "SynchronizationProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_SynchronizationProcessErrors_SynchronizationProcessId",
                table: "SynchronizationProcessErrors",
                column: "SynchronizationProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_SynchronizationProcesses_UserId",
                table: "SynchronizationProcesses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SynchronizationProcessUserCancellations_SynchronizationProc~",
                table: "SynchronizationProcessUserCancellations",
                column: "SynchronizationProcessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "IgnorePaths");

            migrationBuilder.DropTable(
                name: "SynchronizationProcessErrors");

            migrationBuilder.DropTable(
                name: "SynchronizationProcessUserCancellations");

            migrationBuilder.DropTable(
                name: "SynchronizationProcesses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
