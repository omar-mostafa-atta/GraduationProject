using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Application.Migrations
{
    /// <inheritdoc />
    public partial class Adding_ColumnToPatientAndNurse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiastolicPressure",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeartRate",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SystolicPressure",
                table: "Patients",
                type: "int",
                nullable: true);
        

            migrationBuilder.AddColumn<string>(
                name: "Government",
                table: "Nurses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiastolicPressure",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "HeartRate",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "SystolicPressure",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Government",
                table: "Nurses");
        }
    }
}
