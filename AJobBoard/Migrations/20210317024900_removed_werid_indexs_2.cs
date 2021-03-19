using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace Jobtransparency.Migrations
{
    public partial class removed_werid_indexs_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobPostings_Company",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_Description",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_Description_Summary_Company",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_Summary",
                table: "JobPostings");

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "JobPostings",
                type: "tsvector",
                nullable: true)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Description", "Summary", "Company" });

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_SearchVector",
                table: "JobPostings",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobPostings_SearchVector",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "JobPostings");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Company",
                table: "JobPostings",
                column: "Company")
                .Annotation("Npgsql:TsVectorConfig", "english");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Description",
                table: "JobPostings",
                column: "Description")
                .Annotation("Npgsql:TsVectorConfig", "english");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Description_Summary_Company",
                table: "JobPostings",
                columns: new[] { "Description", "Summary", "Company" })
                .Annotation("Npgsql:TsVectorConfig", "english");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Summary",
                table: "JobPostings",
                column: "Summary")
                .Annotation("Npgsql:TsVectorConfig", "english");
        }
    }
}
