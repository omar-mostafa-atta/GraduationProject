using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicationUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Medications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "Medications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Medications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Medications_DoctorId",
                table: "Medications",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Doctors_DoctorId",
                table: "Medications",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Doctors_DoctorId",
                table: "Medications");

            migrationBuilder.DropIndex(
                name: "IX_Medications_DoctorId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Medications");
        }
    }
}
