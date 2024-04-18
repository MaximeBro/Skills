using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class AddsCvSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbstractSkillModel_CVs_CvId",
                table: "AbstractSkillModel");

            migrationBuilder.DropForeignKey(
                name: "FK_AbstractSkillModel_CVs_CvInfoId",
                table: "AbstractSkillModel");

            migrationBuilder.DropForeignKey(
                name: "FK_AbstractSkillModel_CVs_SoftSkillModel_CvInfoId",
                table: "AbstractSkillModel");

            migrationBuilder.DropIndex(
                name: "IX_AbstractSkillModel_CvId",
                table: "AbstractSkillModel");

            migrationBuilder.DropColumn(
                name: "Granted",
                table: "CvSafetyCertifications");

            migrationBuilder.DropColumn(
                name: "CvId",
                table: "AbstractSkillModel");

            migrationBuilder.CreateTable(
                name: "CvSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CvId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SkillId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsSoftSkill = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CvSkills_AbstractSkillModel_SkillId",
                        column: x => x.SkillId,
                        principalTable: "AbstractSkillModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CvSkills_CVs_CvId",
                        column: x => x.CvId,
                        principalTable: "CVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CvSkills_CvId",
                table: "CvSkills",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_CvSkills_SkillId",
                table: "CvSkills",
                column: "SkillId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbstractSkillModel_CVs_CvInfoId",
                table: "AbstractSkillModel");

            migrationBuilder.DropForeignKey(
                name: "FK_AbstractSkillModel_CVs_SoftSkillModel_CvInfoId",
                table: "AbstractSkillModel");

            migrationBuilder.DropTable(
                name: "CvSkills");

            migrationBuilder.AddColumn<bool>(
                name: "Granted",
                table: "CvSafetyCertifications",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "CvId",
                table: "AbstractSkillModel",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbstractSkillModel_CvId",
                table: "AbstractSkillModel",
                column: "CvId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbstractSkillModel_CVs_CvId",
                table: "AbstractSkillModel",
                column: "CvId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AbstractSkillModel_CVs_CvInfoId",
                table: "AbstractSkillModel",
                column: "CvInfoId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AbstractSkillModel_CVs_SoftSkillModel_CvInfoId",
                table: "AbstractSkillModel",
                column: "SoftSkillModel_CvInfoId",
                principalTable: "CVs",
                principalColumn: "Id");
        }
    }
}
