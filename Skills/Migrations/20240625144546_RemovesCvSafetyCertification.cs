using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class RemovesCvSafetyCertification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CvSafetyCertifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CvSafetyCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CertId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CvId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvSafetyCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CvSafetyCertifications_CVs_CvId",
                        column: x => x.CvId,
                        principalTable: "CVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CvSafetyCertifications_SafetyCertifications_CertId",
                        column: x => x.CertId,
                        principalTable: "SafetyCertifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CvSafetyCertifications_CertId",
                table: "CvSafetyCertifications",
                column: "CertId");

            migrationBuilder.CreateIndex(
                name: "IX_CvSafetyCertifications_CvId",
                table: "CvSafetyCertifications",
                column: "CvId");
        }
    }
}
