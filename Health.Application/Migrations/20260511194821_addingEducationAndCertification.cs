using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Application.Migrations
{
    /// <inheritdoc />
    public partial class addingEducationAndCertification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Certification",
                table: "Nurses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "Nurses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Certification",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Certification",
                table: "Nurses");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "Nurses");

            migrationBuilder.DropColumn(
                name: "Certification",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "Doctors");
        }
    }
}
