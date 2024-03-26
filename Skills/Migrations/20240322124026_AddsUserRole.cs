using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class AddsUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Skills",
                newName: "TypeId");

            migrationBuilder.RenameColumn(
                name: "SubCategory",
                table: "Skills",
                newName: "SubCategoryId");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Skills",
                newName: "CategoryId");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SKillInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SKillInfo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skills_CategoryId",
                table: "Skills",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_SubCategoryId",
                table: "Skills",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_TypeId",
                table: "Skills",
                column: "TypeId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropTable(
                name: "SKillInfo");

            migrationBuilder.DropIndex(
                name: "IX_Skills_CategoryId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_SubCategoryId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_TypeId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "Skills",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "SubCategoryId",
                table: "Skills",
                newName: "SubCategory");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Skills",
                newName: "Category");
        }
    }
}
