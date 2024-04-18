using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesSafetyCertificationPropName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "SafetyCertifications",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "SafetyCertifications",
                newName: "Title");
        }
    }
}
