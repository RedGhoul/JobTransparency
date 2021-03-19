using Microsoft.EntityFrameworkCore.Migrations;

namespace Jobtransparency.Migrations
{
    public partial class removed_werid_indexs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_KeyPhrase_Affinty",
                table: "KeyPhrase");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_Title",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_URL",
                table: "JobPostings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_KeyPhrase_Affinty",
                table: "KeyPhrase",
                column: "Affinty");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Title",
                table: "JobPostings",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_URL",
                table: "JobPostings",
                column: "URL");
        }
    }
}
