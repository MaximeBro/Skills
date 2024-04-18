using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesSafetyCertification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "SafetyCertifications",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "SafetyCertifications");
        }
    }
}
