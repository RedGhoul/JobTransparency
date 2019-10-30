using Microsoft.EntityFrameworkCore.Migrations;

namespace AJobBoard.Migrations
{
    public partial class mergepatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "Document",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "Document");
        }
    }
}
