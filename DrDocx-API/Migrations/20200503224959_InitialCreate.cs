using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DrDocx.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AddColumn<int>(name: "PatientId", table: "TestResultGroups", nullable: false);
            migrationBuilder.AddColumn<int>(name: "PatientId", table: "TestResults", nullable: false, defaultValue: 2);
            // migrationBuilder.CreateIndex(
            //     name: "IX_TestResultGroups_PatientId",
            //     table: "TestResultGroups",
            //     column: "PatientId");
            
            migrationBuilder.CreateIndex(
                name: "IX_TestResults_PatientId",
                table: "TestResults",
                column: "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
