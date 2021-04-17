using Microsoft.EntityFrameworkCore.Migrations;

namespace Jobtransparency.Migrations
{
    public partial class JobGetterSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobGettingConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaxAge = table.Column<int>(type: "int", nullable: false),
                    MaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Host = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkCheckIfJobExists = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkAzureFunction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkJobPostingCreation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobGettingConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobGettingConfigId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobType_JobGettingConfig_JobGettingConfigId",
                        column: x => x.JobGettingConfigId,
                        principalTable: "JobGettingConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PositionCities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobGettingConfigId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionCities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionCities_JobGettingConfig_JobGettingConfigId",
                        column: x => x.JobGettingConfigId,
                        principalTable: "JobGettingConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PositionName",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<int>(type: "int", nullable: false),
                    JobGettingConfigId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionName_JobGettingConfig_JobGettingConfigId",
                        column: x => x.JobGettingConfigId,
                        principalTable: "JobGettingConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobType_JobGettingConfigId",
                table: "JobType",
                column: "JobGettingConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionCities_JobGettingConfigId",
                table: "PositionCities",
                column: "JobGettingConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionName_JobGettingConfigId",
                table: "PositionName",
                column: "JobGettingConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobType");

            migrationBuilder.DropTable(
                name: "PositionCities");

            migrationBuilder.DropTable(
                name: "PositionName");

            migrationBuilder.DropTable(
                name: "JobGettingConfig");
        }
    }
}
