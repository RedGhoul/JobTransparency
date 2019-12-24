using Microsoft.EntityFrameworkCore.Migrations;

namespace AJobBoard.Migrations
{
    public partial class AddedSummaryData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SummaryData",
                table: "JobPostings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SummaryData",
                table: "JobPostings");
        }
    }
}
