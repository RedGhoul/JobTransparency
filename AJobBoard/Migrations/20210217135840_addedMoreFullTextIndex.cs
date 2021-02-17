using Microsoft.EntityFrameworkCore.Migrations;

namespace Jobtransparency.Migrations
{
    public partial class addedMoreFullTextIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Company",
                table: "JobPostings",
                column: "Company")
                .Annotation("MySql:FullTextIndex", true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Description",
                table: "JobPostings",
                column: "Description")
                .Annotation("MySql:FullTextIndex", true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Summary",
                table: "JobPostings",
                column: "Summary")
                .Annotation("MySql:FullTextIndex", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobPostings_Company",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_Description",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_Summary",
                table: "JobPostings");
        }
    }
}
