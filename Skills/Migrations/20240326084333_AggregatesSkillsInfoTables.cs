using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class AggregatesSkillsInfoTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SKillInfo_CategoryId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SKillInfo_SubCategoryId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SKillInfo_TypeId",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SKillInfo",
                table: "SKillInfo");

            migrationBuilder.RenameTable(
                name: "SKillInfo",
                newName: "SkillsTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillsTypes",
                table: "SkillsTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SkillsTypes_CategoryId",
                table: "Skills",
                column: "CategoryId",
                principalTable: "SkillsTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SkillsTypes_SubCategoryId",
                table: "Skills",
                column: "SubCategoryId",
                principalTable: "SkillsTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SkillsTypes_TypeId",
                table: "Skills",
                column: "TypeId",
                principalTable: "SkillsTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillsTypes_CategoryId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillsTypes_SubCategoryId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillsTypes_TypeId",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillsTypes",
                table: "SkillsTypes");

            migrationBuilder.RenameTable(
                name: "SkillsTypes",
                newName: "SKillInfo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SKillInfo",
                table: "SKillInfo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SKillInfo_CategoryId",
                table: "Skills",
                column: "CategoryId",
                principalTable: "SKillInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SKillInfo_SubCategoryId",
                table: "Skills",
                column: "SubCategoryId",
                principalTable: "SKillInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SKillInfo_TypeId",
                table: "Skills",
                column: "TypeId",
                principalTable: "SKillInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
