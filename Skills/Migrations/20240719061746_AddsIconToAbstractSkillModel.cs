using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class AddsIconToAbstractSkillModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "AbstractSkillModel",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IconType",
                table: "AbstractSkillModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "AbstractSkillModel");

            migrationBuilder.DropColumn(
                name: "IconType",
                table: "AbstractSkillModel");
        }
    }
}
