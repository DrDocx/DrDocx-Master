using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DrDocx_Core.Migrations
{
    public partial class CreateDbTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    PreferredName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Medications = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    DateOfTesting = table.Column<DateTime>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    MedicalRecordNumber = table.Column<int>(nullable: false),
                    AgeAtTesting = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PatientInfoId = table.Column<int>(nullable: true),
                    Diagnosis = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_PatientInfo_PatientInfoId",
                        column: x => x.PatientInfoId,
                        principalTable: "PatientInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestGroupTests",
                columns: table => new
                {
                    TestId = table.Column<int>(nullable: false),
                    TestGroupId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestGroupTests", x => new { x.TestGroupId, x.TestId });
                    table.ForeignKey(
                        name: "FK_TestGroupTests_TestGroups_TestGroupId",
                        column: x => x.TestGroupId,
                        principalTable: "TestGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestGroupTests_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResultGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestGroupInfoId = table.Column<int>(nullable: true),
                    PatientId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResultGroups_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResultGroups_TestGroups_TestGroupInfoId",
                        column: x => x.TestGroupInfoId,
                        principalTable: "TestGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RawScore = table.Column<int>(nullable: false),
                    ScaledScore = table.Column<int>(nullable: false),
                    ZScore = table.Column<int>(nullable: false),
                    Percentile = table.Column<int>(nullable: false),
                    RelatedTestId = table.Column<int>(nullable: true),
                    TestResultGroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResults_Tests_RelatedTestId",
                        column: x => x.RelatedTestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResults_TestResultGroups_TestResultGroupId",
                        column: x => x.TestResultGroupId,
                        principalTable: "TestResultGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PatientInfoId",
                table: "Patients",
                column: "PatientInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_TestGroupTests_TestId",
                table: "TestGroupTests",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultGroups_PatientId",
                table: "TestResultGroups",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultGroups_TestGroupInfoId",
                table: "TestResultGroups",
                column: "TestGroupInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_RelatedTestId",
                table: "TestResults",
                column: "RelatedTestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestResultGroupId",
                table: "TestResults",
                column: "TestResultGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestGroupTests");

            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "TestResultGroups");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "TestGroups");

            migrationBuilder.DropTable(
                name: "PatientInfo");
        }
    }
}
