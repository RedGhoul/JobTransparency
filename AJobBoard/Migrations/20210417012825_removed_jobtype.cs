using Microsoft.EntityFrameworkCore.Migrations;

namespace Jobtransparency.Migrations
{
    public partial class removed_jobtype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobGettingConfigId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_JobType_JobGettingConfigId",
                table: "JobType",
                column: "JobGettingConfigId");
        }
    }
}
