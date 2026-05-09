using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicalRecordUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MedicalRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "MedicalRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "MedicalRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_DoctorId",
                table: "MedicalRecords",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Doctors_DoctorId",
                table: "MedicalRecords",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_Doctors_DoctorId",
                table: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_DoctorId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "MedicalRecords");
        }
    }
}
