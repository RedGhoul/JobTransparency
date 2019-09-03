using Microsoft.EntityFrameworkCore.Migrations;

namespace AJobBoard.Data.Migrations
{
    public partial class updatedJobPostingModelwithtelemetry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobSource",
                table: "JobPostings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfApplies",
                table: "JobPostings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfViews",
                table: "JobPostings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobSource",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "NumberOfApplies",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "NumberOfViews",
                table: "JobPostings");
        }
    }
}
