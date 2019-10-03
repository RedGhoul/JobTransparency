using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace AJobBoard.Migrations
{
    public partial class addedapplies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PosterId",
                table: "JobPostings",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Applies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    JobPostingId = table.Column<int>(nullable: true),
                    ApplierId = table.Column<string>(nullable: true),
                    DateAddedToApplies = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applies_AspNetUsers_ApplierId",
                        column: x => x.ApplierId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applies_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_PosterId",
                table: "JobPostings",
                column: "PosterId");

            migrationBuilder.CreateIndex(
                name: "IX_Applies_ApplierId",
                table: "Applies",
                column: "ApplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Applies_JobPostingId",
                table: "Applies",
                column: "JobPostingId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_AspNetUsers_PosterId",
                table: "JobPostings",
                column: "PosterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_AspNetUsers_PosterId",
                table: "JobPostings");

            migrationBuilder.DropTable(
                name: "Applies");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_PosterId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "PosterId",
                table: "JobPostings");
        }
    }
}
