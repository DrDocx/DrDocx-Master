using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DrDocx.API.Migrations
{
    public partial class RemoveScoreTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("Tests", null, "OldTests");
            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    TestGroupId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tests_TestGroups_TestGroupId",
                        column: x => x.TestGroupId,
                        principalTable: "TestGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    
                });
            migrationBuilder.Sql(@"INSERT INTO Tests (Id, Name, Description)
                                   SELECT Id, Name, Description
                                   FROM OldTests
                                ");

            migrationBuilder.DropTable(
                name: "ScoreType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
