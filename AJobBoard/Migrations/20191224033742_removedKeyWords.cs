using Microsoft.EntityFrameworkCore.Migrations;

namespace AJobBoard.Migrations
{
    public partial class removedKeyWords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyWords",
                table: "JobPostings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeyWords",
                table: "JobPostings",
                nullable: true);
        }
    }
}
