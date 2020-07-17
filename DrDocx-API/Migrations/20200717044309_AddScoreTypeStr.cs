using Microsoft.EntityFrameworkCore.Migrations;

namespace DrDocx.API.Migrations
{
    public partial class AddScoreTypeStr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScoreType",
                table: "Tests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
