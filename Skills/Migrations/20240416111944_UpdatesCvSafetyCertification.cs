using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesCvSafetyCertification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
