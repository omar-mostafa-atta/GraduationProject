using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Application.Migrations
{
    /// <inheritdoc />
    public partial class Adding_Sugar_ColumnToPatientAndNurse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sugar",
                table: "Patients",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sugar",
                table: "Patients");
        }
    }
}
