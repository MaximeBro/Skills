using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class AddsSkillInfoIconType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "AbstractSkillModel");

            migrationBuilder.DropColumn(
                name: "IconType",
                table: "AbstractSkillModel");

            migrationBuilder.AddColumn<int>(
                name: "IconType",
                table: "SkillsTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconType",
                table: "SkillsTypes");

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
    }
}
