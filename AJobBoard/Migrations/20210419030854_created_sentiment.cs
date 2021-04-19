using Microsoft.EntityFrameworkCore.Migrations;

namespace Jobtransparency.Migrations
{
    public partial class created_sentiment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sentiment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pos = table.Column<float>(type: "real", nullable: false),
                    compound = table.Column<float>(type: "real", nullable: false),
                    neu = table.Column<float>(type: "real", nullable: false),
                    neg = table.Column<float>(type: "real", nullable: false),
                    JobPostingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sentiment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sentiment_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sentiment_JobPostingId",
                table: "Sentiment",
                column: "JobPostingId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sentiment");
        }
    }
}
