using Microsoft.EntityFrameworkCore.Migrations;

namespace Jobtransparency.Migrations
{
    public partial class added_expired_col_jobposting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Expried",
                table: "JobPostings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expried",
                table: "JobPostings");
        }
    }
}
