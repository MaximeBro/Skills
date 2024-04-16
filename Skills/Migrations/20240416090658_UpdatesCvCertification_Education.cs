using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesCvCertification_Education : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "CvCertifications",
                newName: "Duration");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "CvCertifications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "CvCertifications");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "CvCertifications",
                newName: "Description");
        }
    }
}
