using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class RefactorsSkillsModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Groups_GroupId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillsTypes_CategoryId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillsTypes_SubCategoryId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SkillsTypes_TypeId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Userskills_Skills_SkillId",
                table: "Userskills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Userskills",
                table: "Userskills");

            migrationBuilder.DropIndex(
                name: "IX_Userskills_UserId",
                table: "Userskills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skills",
                table: "Skills");

            migrationBuilder.RenameTable(
                name: "Skills",
                newName: "AbstractSkillModel");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_TypeId",
                table: "AbstractSkillModel",
                newName: "IX_AbstractSkillModel_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_SubCategoryId",
                table: "AbstractSkillModel",
                newName: "IX_AbstractSkillModel_SubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_GroupId",
                table: "AbstractSkillModel",
                newName: "IX_AbstractSkillModel_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_CategoryId",
                table: "AbstractSkillModel",
                newName: "IX_AbstractSkillModel_CategoryId");

            migrationBuilder.AddColumn<bool>(
                name: "IsSoftSkill",
                table: "Userskills",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "AbstractSkillModel",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "AbstractSkillModel",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AbstractSkillModel",
                type: "TEXT",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubCategory",
                table: "AbstractSkillModel",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "AbstractSkillModel",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Userskills",
                table: "Userskills",
                columns: new[] { "UserId", "SkillId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AbstractSkillModel",
                table: "AbstractSkillModel",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SoftTypesLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SkillId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftTypesLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoftTypesLevels_AbstractSkillModel_SkillId",
                        column: x => x.SkillId,
                        principalTable: "AbstractSkillModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Userskills_SkillId",
                table: "Userskills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_SoftTypesLevels_SkillId",
                table: "SoftTypesLevels",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbstractSkillModel_Groups_GroupId",
                table: "AbstractSkillModel",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AbstractSkillModel_SkillsTypes_CategoryId",
                table: "AbstractSkillModel",
                column: "CategoryId",
                principalTable: "SkillsTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AbstractSkillModel_SkillsTypes_SubCategoryId",
                table: "AbstractSkillModel",
                column: "SubCategoryId",
                principalTable: "SkillsTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AbstractSkillModel_SkillsTypes_TypeId",
                table: "AbstractSkillModel",
                column: "TypeId",
                principalTable: "SkillsTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Userskills_AbstractSkillModel_SkillId",
                table: "Userskills",
                column: "SkillId",
                principalTable: "AbstractSkillModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbstractSkillModel_Groups_GroupId",
                table: "AbstractSkillModel");

            migrationBuilder.DropForeignKey(
                name: "FK_AbstractSkillModel_SkillsTypes_CategoryId",
                table: "AbstractSkillModel");

            migrationBuilder.DropForeignKey(
                name: "FK_AbstractSkillModel_SkillsTypes_SubCategoryId",
                table: "AbstractSkillModel");

            migrationBuilder.DropForeignKey(
                name: "FK_AbstractSkillModel_SkillsTypes_TypeId",
                table: "AbstractSkillModel");

            migrationBuilder.DropForeignKey(
                name: "FK_Userskills_AbstractSkillModel_SkillId",
                table: "Userskills");

            migrationBuilder.DropTable(
                name: "SoftTypesLevels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Userskills",
                table: "Userskills");

            migrationBuilder.DropIndex(
                name: "IX_Userskills_SkillId",
                table: "Userskills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AbstractSkillModel",
                table: "AbstractSkillModel");

            migrationBuilder.DropColumn(
                name: "IsSoftSkill",
                table: "Userskills");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "AbstractSkillModel");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AbstractSkillModel");

            migrationBuilder.DropColumn(
                name: "SubCategory",
                table: "AbstractSkillModel");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AbstractSkillModel");

            migrationBuilder.RenameTable(
                name: "AbstractSkillModel",
                newName: "Skills");

            migrationBuilder.RenameIndex(
                name: "IX_AbstractSkillModel_TypeId",
                table: "Skills",
                newName: "IX_Skills_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_AbstractSkillModel_SubCategoryId",
                table: "Skills",
                newName: "IX_Skills_SubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_AbstractSkillModel_GroupId",
                table: "Skills",
                newName: "IX_Skills_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_AbstractSkillModel_CategoryId",
                table: "Skills",
                newName: "IX_Skills_CategoryId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "Skills",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Userskills",
                table: "Userskills",
                columns: new[] { "SkillId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skills",
                table: "Skills",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Userskills_UserId",
                table: "Userskills",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Groups_GroupId",
                table: "Skills",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Userskills_Skills_SkillId",
                table: "Userskills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
