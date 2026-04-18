using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Application.Migrations
{
    /// <inheritdoc />
    public partial class addingCalendyColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CalendlyAccessToken",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalendlyOrganizationUri",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalendlyRefreshToken",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalendlySchedulingUrl",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalendlyUri",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalendlyEventUri",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalendlyJoinUrl",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalendlyAccessToken",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "CalendlyOrganizationUri",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "CalendlyRefreshToken",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "CalendlySchedulingUrl",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "CalendlyUri",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "CalendlyEventUri",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CalendlyJoinUrl",
                table: "Appointments");
        }
    }
}
