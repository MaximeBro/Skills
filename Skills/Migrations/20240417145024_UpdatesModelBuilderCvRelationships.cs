using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesModelBuilderCvRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CvCertifications_CVs_CvInfoId",
                table: "CvCertifications");

            migrationBuilder.DropForeignKey(
                name: "FK_CvEducations_CVs_CvInfoId",
                table: "CvEducations");

            migrationBuilder.DropForeignKey(
                name: "FK_CvExperiences_CVs_CvInfoId",
                table: "CvExperiences");

            migrationBuilder.DropForeignKey(
                name: "FK_CvSafetyCertifications_CVs_CvInfoId",
                table: "CvSafetyCertifications");

            migrationBuilder.DropForeignKey(
                name: "FK_CvSafetyCertifications_SafetyCertifications_CertId",
                table: "CvSafetyCertifications");

            migrationBuilder.DropForeignKey(
                name: "FK_CvSkills_CVs_CvInfoId",
                table: "CvSkills");

            migrationBuilder.DropIndex(
                name: "IX_CvSkills_CvInfoId",
                table: "CvSkills");

            migrationBuilder.DropIndex(
                name: "IX_CvSafetyCertifications_CertId",
                table: "CvSafetyCertifications");

            migrationBuilder.DropIndex(
                name: "IX_CvExperiences_CvInfoId",
                table: "CvExperiences");

            migrationBuilder.DropIndex(
                name: "IX_CvEducations_CvInfoId",
                table: "CvEducations");

            migrationBuilder.DropIndex(
                name: "IX_CvCertifications_CvInfoId",
                table: "CvCertifications");

            migrationBuilder.DropColumn(
                name: "CvInfoId",
                table: "CvSkills");

            migrationBuilder.DropColumn(
                name: "CvInfoId",
                table: "CvExperiences");

            migrationBuilder.DropColumn(
                name: "CvInfoId",
                table: "CvEducations");

            migrationBuilder.DropColumn(
                name: "CvInfoId",
                table: "CvCertifications");

            migrationBuilder.RenameColumn(
                name: "CvInfoId",
                table: "CvSafetyCertifications",
                newName: "CertificationId");

            migrationBuilder.RenameIndex(
                name: "IX_CvSafetyCertifications_CvInfoId",
                table: "CvSafetyCertifications",
                newName: "IX_CvSafetyCertifications_CertificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CvSafetyCertifications_SafetyCertifications_CertificationId",
                table: "CvSafetyCertifications",
                column: "CertificationId",
                principalTable: "SafetyCertifications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CvSafetyCertifications_SafetyCertifications_CertificationId",
                table: "CvSafetyCertifications");

            migrationBuilder.RenameColumn(
                name: "CertificationId",
                table: "CvSafetyCertifications",
                newName: "CvInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_CvSafetyCertifications_CertificationId",
                table: "CvSafetyCertifications",
                newName: "IX_CvSafetyCertifications_CvInfoId");

            migrationBuilder.AddColumn<Guid>(
                name: "CvInfoId",
                table: "CvSkills",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CvInfoId",
                table: "CvExperiences",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CvInfoId",
                table: "CvEducations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CvInfoId",
                table: "CvCertifications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CvSkills_CvInfoId",
                table: "CvSkills",
                column: "CvInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_CvSafetyCertifications_CertId",
                table: "CvSafetyCertifications",
                column: "CertId");

            migrationBuilder.CreateIndex(
                name: "IX_CvExperiences_CvInfoId",
                table: "CvExperiences",
                column: "CvInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_CvEducations_CvInfoId",
                table: "CvEducations",
                column: "CvInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_CvCertifications_CvInfoId",
                table: "CvCertifications",
                column: "CvInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_CvCertifications_CVs_CvInfoId",
                table: "CvCertifications",
                column: "CvInfoId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CvEducations_CVs_CvInfoId",
                table: "CvEducations",
                column: "CvInfoId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CvExperiences_CVs_CvInfoId",
                table: "CvExperiences",
                column: "CvInfoId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CvSafetyCertifications_CVs_CvInfoId",
                table: "CvSafetyCertifications",
                column: "CvInfoId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CvSafetyCertifications_SafetyCertifications_CertId",
                table: "CvSafetyCertifications",
                column: "CertId",
                principalTable: "SafetyCertifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CvSkills_CVs_CvInfoId",
                table: "CvSkills",
                column: "CvInfoId",
                principalTable: "CVs",
                principalColumn: "Id");
        }
    }
}
