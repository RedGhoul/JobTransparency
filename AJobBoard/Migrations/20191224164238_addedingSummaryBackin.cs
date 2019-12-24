using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AJobBoard.Migrations
{
    public partial class addedingSummaryBackin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SummaryData");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "JobPostings",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "KeyPhrase",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Affinty = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    JobPostingId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyPhrase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyPhrase_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeyPhrase_JobPostingId",
                table: "KeyPhrase",
                column: "JobPostingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyPhrase");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "JobPostings");

            migrationBuilder.CreateTable(
                name: "SummaryData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Affinty = table.Column<string>(nullable: true),
                    JobPostingId = table.Column<int>(nullable: true),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummaryData_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SummaryData_JobPostingId",
                table: "SummaryData",
                column: "JobPostingId");
        }
    }
}
