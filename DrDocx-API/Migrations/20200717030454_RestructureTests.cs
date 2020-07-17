using Microsoft.EntityFrameworkCore.Migrations;

namespace DrDocx.API.Migrations
{
    public partial class RestructureTests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestGroupTests");

            migrationBuilder.RenameTable("Tests", null, "OldTests");
            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    StandardizedScoreTypeId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    TestGroupId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tests_ScoreType_StandardizedScoreTypeId",
                        column: x => x.StandardizedScoreTypeId,
                        principalTable: "ScoreType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tests_TestGroups_TestGroupId",
                        column: x => x.TestGroupId,
                        principalTable: "TestGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    
                });
            migrationBuilder.Sql(@"INSERT INTO Tests (Id, Name, StandardizedScoreTypeId, Description)
                                   SELECT Id, Name, StandardizedScoreTypeId, Description
                                   FROM OldTests
                                ");

            migrationBuilder.DropTable("OldTests");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_TestGroupId",
                table: "Tests",
                column: "TestGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
