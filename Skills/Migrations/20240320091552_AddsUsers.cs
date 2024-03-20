using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class AddsUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSkillModel_Skills_SkillsId",
                table: "UserSkillModel");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSkillModel_UserModel_UsersId",
                table: "UserSkillModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSkillModel",
                table: "UserSkillModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserModel",
                table: "UserModel");

            migrationBuilder.RenameTable(
                name: "UserSkillModel",
                newName: "Userskills");

            migrationBuilder.RenameTable(
                name: "UserModel",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_UserSkillModel_UsersId",
                table: "Userskills",
                newName: "IX_Userskills_UsersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Userskills",
                table: "Userskills",
                columns: new[] { "SkillsId", "UsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Userskills_Skills_SkillsId",
                table: "Userskills",
                column: "SkillsId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Userskills_Users_UsersId",
                table: "Userskills",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Userskills_Skills_SkillsId",
                table: "Userskills");

            migrationBuilder.DropForeignKey(
                name: "FK_Userskills_Users_UsersId",
                table: "Userskills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Userskills",
                table: "Userskills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Userskills",
                newName: "UserSkillModel");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "UserModel");

            migrationBuilder.RenameIndex(
                name: "IX_Userskills_UsersId",
                table: "UserSkillModel",
                newName: "IX_UserSkillModel_UsersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSkillModel",
                table: "UserSkillModel",
                columns: new[] { "SkillsId", "UsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserModel",
                table: "UserModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkillModel_Skills_SkillsId",
                table: "UserSkillModel",
                column: "SkillsId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkillModel_UserModel_UsersId",
                table: "UserSkillModel",
                column: "UsersId",
                principalTable: "UserModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
