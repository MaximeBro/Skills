using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesCvEducationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Year",
                table: "CvEducations",
                newName: "YearStart");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CvEducations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "CvEducations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Town",
                table: "CvEducations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "YearEnd",
                table: "CvEducations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "CvEducations");

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "CvEducations");

            migrationBuilder.DropColumn(
                name: "Town",
                table: "CvEducations");

            migrationBuilder.DropColumn(
                name: "YearEnd",
                table: "CvEducations");

            migrationBuilder.RenameColumn(
                name: "YearStart",
                table: "CvEducations",
                newName: "Year");
        }
    }
}
