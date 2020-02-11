using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DrDocx_Core.Migrations
{
    public partial class ConsolidatePatientInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Patients",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AgeAtTesting",
                table: "Patients",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Patients",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfTesting",
                table: "Patients",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MedicalRecordNumber",
                table: "Patients",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Medications",
                table: "Patients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Patients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Patients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredName",
                table: "Patients",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "AgeAtTesting",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DateOfTesting",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "MedicalRecordNumber",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Medications",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PreferredName",
                table: "Patients");

            migrationBuilder.AddColumn<int>(
                name: "PatientInfoId",
                table: "Patients",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PatientInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    AgeAtTesting = table.Column<int>(type: "INTEGER", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateOfTesting = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MedicalRecordNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Medications = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    PreferredName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientInfo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PatientInfoId",
                table: "Patients",
                column: "PatientInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_PatientInfo_PatientInfoId",
                table: "Patients",
                column: "PatientInfoId",
                principalTable: "PatientInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
