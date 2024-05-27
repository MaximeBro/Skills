using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesCvModelBuilder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CvCertifications",
                table: "CvCertifications");

            migrationBuilder.DropIndex(
                name: "IX_CvCertifications_CvId",
                table: "CvCertifications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CvCertifications",
                table: "CvCertifications",
                columns: new[] { "CvId", "CertificationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CvCertifications_CertificationId",
                table: "CvCertifications",
                column: "CertificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CvCertifications",
                table: "CvCertifications");

            migrationBuilder.DropIndex(
                name: "IX_CvCertifications_CertificationId",
                table: "CvCertifications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CvCertifications",
                table: "CvCertifications",
                columns: new[] { "CertificationId", "CvId" });

            migrationBuilder.CreateIndex(
                name: "IX_CvCertifications_CvId",
                table: "CvCertifications",
                column: "CvId");
        }
    }
}
