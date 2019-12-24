using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AJobBoard.Migrations
{
    public partial class madeAlotOfChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Document_AspNetUsers_ApplicationUserId",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "SummaryData",
                table: "JobPostings");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Document",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Document_ApplicationUserId",
                table: "Document",
                newName: "IX_Document_OwnerId");

            migrationBuilder.CreateTable(
                name: "SummaryData",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Document_AspNetUsers_OwnerId",
                table: "Document",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Document_AspNetUsers_OwnerId",
                table: "Document");

            migrationBuilder.DropTable(
                name: "SummaryData");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Document",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Document_OwnerId",
                table: "Document",
                newName: "IX_Document_ApplicationUserId");

            migrationBuilder.AddColumn<string>(
                name: "SummaryData",
                table: "JobPostings",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Document_AspNetUsers_ApplicationUserId",
                table: "Document",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
