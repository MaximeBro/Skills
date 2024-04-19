using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class AddsSafetyCertifModelBuilderConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CvSafetyCertifications_SafetyCertifications_CertificationId",
                table: "CvSafetyCertifications");

            migrationBuilder.DropIndex(
                name: "IX_CvSafetyCertifications_CertificationId",
                table: "CvSafetyCertifications");

            migrationBuilder.DropColumn(
                name: "CertificationId",
                table: "CvSafetyCertifications");

            migrationBuilder.CreateIndex(
                name: "IX_CvSafetyCertifications_CertId",
                table: "CvSafetyCertifications",
                column: "CertId");

            migrationBuilder.AddForeignKey(
                name: "FK_CvSafetyCertifications_SafetyCertifications_CertId",
                table: "CvSafetyCertifications",
                column: "CertId",
                principalTable: "SafetyCertifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CvSafetyCertifications_SafetyCertifications_CertId",
                table: "CvSafetyCertifications");

            migrationBuilder.DropIndex(
                name: "IX_CvSafetyCertifications_CertId",
                table: "CvSafetyCertifications");

            migrationBuilder.AddColumn<Guid>(
                name: "CertificationId",
                table: "CvSafetyCertifications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CvSafetyCertifications_CertificationId",
                table: "CvSafetyCertifications",
                column: "CertificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CvSafetyCertifications_SafetyCertifications_CertificationId",
                table: "CvSafetyCertifications",
                column: "CertificationId",
                principalTable: "SafetyCertifications",
                principalColumn: "Id");
        }
    }
}
