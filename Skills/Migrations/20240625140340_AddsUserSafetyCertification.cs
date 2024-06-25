using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class AddsUserSafetyCertification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserSafetyCertificationInfoId",
                table: "CVs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserSafetyCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CertId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CertificationId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSafetyCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSafetyCertifications_SafetyCertifications_CertificationId",
                        column: x => x.CertificationId,
                        principalTable: "SafetyCertifications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserSafetyCertifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CVs_UserSafetyCertificationInfoId",
                table: "CVs",
                column: "UserSafetyCertificationInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSafetyCertifications_CertificationId",
                table: "UserSafetyCertifications",
                column: "CertificationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSafetyCertifications_UserId",
                table: "UserSafetyCertifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_UserSafetyCertifications_UserSafetyCertificationInfoId",
                table: "CVs",
                column: "UserSafetyCertificationInfoId",
                principalTable: "UserSafetyCertifications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CVs_UserSafetyCertifications_UserSafetyCertificationInfoId",
                table: "CVs");

            migrationBuilder.DropTable(
                name: "UserSafetyCertifications");

            migrationBuilder.DropIndex(
                name: "IX_CVs_UserSafetyCertificationInfoId",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "UserSafetyCertificationInfoId",
                table: "CVs");
        }
    }
}
