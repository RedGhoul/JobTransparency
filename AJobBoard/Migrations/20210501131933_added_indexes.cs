using Microsoft.EntityFrameworkCore.Migrations;

namespace Jobtransparency.Migrations
{
    public partial class added_indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Sentiment_compound",
                table: "Sentiment",
                column: "compound");

            migrationBuilder.CreateIndex(
                name: "IX_Sentiment_neg",
                table: "Sentiment",
                column: "neg");

            migrationBuilder.CreateIndex(
                name: "IX_Sentiment_pos",
                table: "Sentiment",
                column: "pos");

            migrationBuilder.CreateIndex(
                name: "IX_KeyPhrase_Affinty",
                table: "KeyPhrase",
                column: "Affinty");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_DateAdded",
                table: "JobPostings",
                column: "DateAdded");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sentiment_compound",
                table: "Sentiment");

            migrationBuilder.DropIndex(
                name: "IX_Sentiment_neg",
                table: "Sentiment");

            migrationBuilder.DropIndex(
                name: "IX_Sentiment_pos",
                table: "Sentiment");

            migrationBuilder.DropIndex(
                name: "IX_KeyPhrase_Affinty",
                table: "KeyPhrase");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_DateAdded",
                table: "JobPostings");
        }
    }
}
