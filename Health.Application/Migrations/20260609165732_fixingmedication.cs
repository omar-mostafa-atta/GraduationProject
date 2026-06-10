using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Application.Migrations
{
    /// <inheritdoc />
    public partial class fixingmedication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Frequency",
                table: "Medications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "NextReminderTime",
                table: "Medications",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextReminderTime",
                table: "Medications");

            migrationBuilder.AlterColumn<string>(
                name: "Frequency",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
