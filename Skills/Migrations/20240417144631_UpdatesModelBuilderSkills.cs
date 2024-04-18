using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesModelBuilderSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbstractSkillModel_CVs_CvInfoId",
                table: "AbstractSkillModel");

            migrationBuilder.DropForeignKey(
                name: "FK_AbstractSkillModel_CVs_SoftSkillModel_CvInfoId",
                table: "AbstractSkillModel");

            migrationBuilder.DropIndex(
                name: "IX_AbstractSkillModel_CvInfoId",
                table: "AbstractSkillModel");

            migrationBuilder.DropIndex(
                name: "IX_AbstractSkillModel_SoftSkillModel_CvInfoId",
                table: "AbstractSkillModel");

            migrationBuilder.DropColumn(
                name: "CvInfoId",
                table: "AbstractSkillModel");

            migrationBuilder.DropColumn(
                name: "SoftSkillModel_CvInfoId",
                table: "AbstractSkillModel");

            migrationBuilder.AddColumn<Guid>(
                name: "CvInfoId",
                table: "CvSkills",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CvSkills_CvInfoId",
                table: "CvSkills",
                column: "CvInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_CvSkills_CVs_CvInfoId",
                table: "CvSkills",
                column: "CvInfoId",
                principalTable: "CVs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CvSkills_CVs_CvInfoId",
                table: "CvSkills");

            migrationBuilder.DropIndex(
                name: "IX_CvSkills_CvInfoId",
                table: "CvSkills");

            migrationBuilder.DropColumn(
                name: "CvInfoId",
                table: "CvSkills");

            migrationBuilder.AddColumn<Guid>(
                name: "CvInfoId",
                table: "AbstractSkillModel",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SoftSkillModel_CvInfoId",
                table: "AbstractSkillModel",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbstractSkillModel_CvInfoId",
                table: "AbstractSkillModel",
                column: "CvInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_AbstractSkillModel_SoftSkillModel_CvInfoId",
                table: "AbstractSkillModel",
                column: "SoftSkillModel_CvInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbstractSkillModel_CVs_CvInfoId",
                table: "AbstractSkillModel",
                column: "CvInfoId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AbstractSkillModel_CVs_SoftSkillModel_CvInfoId",
                table: "AbstractSkillModel",
                column: "SoftSkillModel_CvInfoId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
