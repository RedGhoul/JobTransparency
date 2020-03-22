using Microsoft.EntityFrameworkCore.Migrations;

namespace AJobBoard.Migrations
{
    public partial class seeing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeyPhrase_JobPostings_JobPostingId",
                table: "KeyPhrase");

            migrationBuilder.AlterColumn<int>(
                name: "JobPostingId",
                table: "KeyPhrase",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_KeyPhrase_JobPostings_JobPostingId",
                table: "KeyPhrase",
                column: "JobPostingId",
                principalTable: "JobPostings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeyPhrase_JobPostings_JobPostingId",
                table: "KeyPhrase");

            migrationBuilder.AlterColumn<int>(
                name: "JobPostingId",
                table: "KeyPhrase",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_KeyPhrase_JobPostings_JobPostingId",
                table: "KeyPhrase",
                column: "JobPostingId",
                principalTable: "JobPostings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
