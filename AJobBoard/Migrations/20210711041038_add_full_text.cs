using Microsoft.EntityFrameworkCore.Migrations;

namespace Jobtransparency.Migrations
{
    public partial class add_full_text : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
               sql: "CREATE FULLTEXT CATALOG FT_Catalog AS DEFAULT;",
               suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT INDEX ON JobPostings(Summary,Title,Company,Location) KEY INDEX PK_JobPostings WITH (CHANGE_TRACKING = AUTO);",
                suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                sql: "DROP FULLTEXT INDEX ON JobPostings",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "DROP FULLTEXT CATALOG FT_Catalog AS DEFAULT;",
                suppressTransaction: true);
        }
    }
}
