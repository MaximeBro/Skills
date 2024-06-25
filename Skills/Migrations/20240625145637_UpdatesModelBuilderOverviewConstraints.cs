using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesModelBuilderOverviewConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSafetyCertifications_SafetyCertifications_CertificationId",
                table: "UserSafetyCertifications");

            migrationBuilder.DropIndex(
                name: "IX_UserSafetyCertifications_CertificationId",
                table: "UserSafetyCertifications");

            migrationBuilder.DropColumn(
                name: "CertificationId",
                table: "UserSafetyCertifications");

            migrationBuilder.CreateIndex(
                name: "IX_UserSafetyCertifications_CertId",
                table: "UserSafetyCertifications",
                column: "CertId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSafetyCertifications_SafetyCertifications_CertId",
                table: "UserSafetyCertifications",
                column: "CertId",
                principalTable: "SafetyCertifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSafetyCertifications_SafetyCertifications_CertId",
                table: "UserSafetyCertifications");

            migrationBuilder.DropIndex(
                name: "IX_UserSafetyCertifications_CertId",
                table: "UserSafetyCertifications");

            migrationBuilder.AddColumn<Guid>(
                name: "CertificationId",
                table: "UserSafetyCertifications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSafetyCertifications_CertificationId",
                table: "UserSafetyCertifications",
                column: "CertificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSafetyCertifications_SafetyCertifications_CertificationId",
                table: "UserSafetyCertifications",
                column: "CertificationId",
                principalTable: "SafetyCertifications",
                principalColumn: "Id");
        }
    }
}
