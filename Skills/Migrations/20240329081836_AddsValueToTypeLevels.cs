using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class AddsValueToTypeLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TypeLevel_SkillsTypes_TypeId",
                table: "TypeLevel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TypeLevel",
                table: "TypeLevel");

            migrationBuilder.RenameTable(
                name: "TypeLevel",
                newName: "TypesLevels");

            migrationBuilder.RenameIndex(
                name: "IX_TypeLevel_TypeId",
                table: "TypesLevels",
                newName: "IX_TypesLevels_TypeId");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "TypesLevels",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TypesLevels",
                table: "TypesLevels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TypesLevels_SkillsTypes_TypeId",
                table: "TypesLevels",
                column: "TypeId",
                principalTable: "SkillsTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TypesLevels_SkillsTypes_TypeId",
                table: "TypesLevels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TypesLevels",
                table: "TypesLevels");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "TypesLevels");

            migrationBuilder.RenameTable(
                name: "TypesLevels",
                newName: "TypeLevel");

            migrationBuilder.RenameIndex(
                name: "IX_TypesLevels_TypeId",
                table: "TypeLevel",
                newName: "IX_TypeLevel_TypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TypeLevel",
                table: "TypeLevel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TypeLevel_SkillsTypes_TypeId",
                table: "TypeLevel",
                column: "TypeId",
                principalTable: "SkillsTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
