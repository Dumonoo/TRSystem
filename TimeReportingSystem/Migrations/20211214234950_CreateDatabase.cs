using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace TimeReportingSystem.Migrations
{
    public partial class CreateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    userId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    userName = table.Column<string>(maxLength: 20, nullable: false),
                    name = table.Column<string>(maxLength: 40, nullable: false),
                    surname = table.Column<string>(maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    activityId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(maxLength: 30, nullable: false),
                    managerId = table.Column<int>(nullable: false),
                    name = table.Column<string>(maxLength: 90, nullable: false),
                    budget = table.Column<int>(nullable: false),
                    active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.activityId);
                    table.ForeignKey(
                        name: "FK_Project_User_managerId",
                        column: x => x.managerId,
                        principalTable: "User",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Raport",
                columns: table => new
                {
                    raportId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    frozen = table.Column<bool>(nullable: false),
                    year = table.Column<int>(nullable: false),
                    month = table.Column<int>(nullable: false),
                    userId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raport", x => x.raportId);
                    table.ForeignKey(
                        name: "FK_Raport_User_userId",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subactivity",
                columns: table => new
                {
                    subactivityId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(nullable: false),
                    activityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subactivity", x => x.subactivityId);
                    table.ForeignKey(
                        name: "FK_Subactivity_Project_activityId",
                        column: x => x.activityId,
                        principalTable: "Project",
                        principalColumn: "activityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcceptedTime",
                columns: table => new
                {
                    acceptedId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    time = table.Column<int>(nullable: false),
                    raportId = table.Column<int>(nullable: false),
                    activityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptedTime", x => x.acceptedId);
                    table.ForeignKey(
                        name: "FK_AcceptedTime_Project_activityId",
                        column: x => x.activityId,
                        principalTable: "Project",
                        principalColumn: "activityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcceptedTime_Raport_raportId",
                        column: x => x.raportId,
                        principalTable: "Raport",
                        principalColumn: "raportId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaportEntry",
                columns: table => new
                {
                    entryId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    date = table.Column<string>(nullable: false),
                    time = table.Column<int>(nullable: false),
                    description = table.Column<string>(maxLength: 2000, nullable: true),
                    activityId = table.Column<int>(nullable: false),
                    subactivityId = table.Column<int>(nullable: false),
                    raportId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaportEntry", x => x.entryId);
                    table.ForeignKey(
                        name: "FK_RaportEntry_Project_activityId",
                        column: x => x.activityId,
                        principalTable: "Project",
                        principalColumn: "activityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaportEntry_Raport_raportId",
                        column: x => x.raportId,
                        principalTable: "Raport",
                        principalColumn: "raportId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaportEntry_Subactivity_subactivityId",
                        column: x => x.subactivityId,
                        principalTable: "Subactivity",
                        principalColumn: "subactivityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedTime_activityId",
                table: "AcceptedTime",
                column: "activityId");

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedTime_raportId",
                table: "AcceptedTime",
                column: "raportId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_managerId",
                table: "Project",
                column: "managerId");

            migrationBuilder.CreateIndex(
                name: "IX_Raport_userId",
                table: "Raport",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_RaportEntry_activityId",
                table: "RaportEntry",
                column: "activityId");

            migrationBuilder.CreateIndex(
                name: "IX_RaportEntry_raportId",
                table: "RaportEntry",
                column: "raportId");

            migrationBuilder.CreateIndex(
                name: "IX_RaportEntry_subactivityId",
                table: "RaportEntry",
                column: "subactivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Subactivity_activityId",
                table: "Subactivity",
                column: "activityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcceptedTime");

            migrationBuilder.DropTable(
                name: "RaportEntry");

            migrationBuilder.DropTable(
                name: "Raport");

            migrationBuilder.DropTable(
                name: "Subactivity");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
