using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesCvEducationExperienceCertification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CvExperiences",
                table: "CvExperiences");

            migrationBuilder.DropIndex(
                name: "IX_CvExperiences_CvId",
                table: "CvExperiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CvEducations",
                table: "CvEducations");

            migrationBuilder.DropIndex(
                name: "IX_CvEducations_CvId",
                table: "CvEducations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CvCertifications",
                table: "CvCertifications");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CvExperiences");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "CvExperiences");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CvExperiences");

            migrationBuilder.DropColumn(
                name: "EndsAt",
                table: "CvExperiences");

            migrationBuilder.DropColumn(
                name: "StartsAt",
                table: "CvExperiences");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CvEducations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CvEducations");

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "CvEducations");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "CvEducations");

            migrationBuilder.DropColumn(
                name: "YearEnd",
                table: "CvEducations");

            migrationBuilder.DropColumn(
                name: "YearStart",
                table: "CvEducations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CvCertifications");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "CvCertifications");

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "CvCertifications");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "CvCertifications");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "CvExperiences",
                newName: "ExperienceId");

            migrationBuilder.RenameColumn(
                name: "Town",
                table: "CvEducations",
                newName: "EducationId");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "CvCertifications",
                newName: "CertificationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CvExperiences",
                table: "CvExperiences",
                columns: new[] { "CvId", "ExperienceId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CvEducations",
                table: "CvEducations",
                columns: new[] { "CvId", "EducationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CvCertifications",
                table: "CvCertifications",
                columns: new[] { "CertificationId", "CvId" });

            migrationBuilder.CreateTable(
                name: "UserCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Supplier = table.Column<string>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Duration = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCertifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEducations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    YearStart = table.Column<int>(type: "INTEGER", nullable: false),
                    YearEnd = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Supplier = table.Column<string>(type: "TEXT", nullable: false),
                    Town = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEducations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEducations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserExperiences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CvExperiences_ExperienceId",
                table: "CvExperiences",
                column: "ExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_CvEducations_EducationId",
                table: "CvEducations",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCertifications_UserId",
                table: "UserCertifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEducations_UserId",
                table: "UserEducations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExperiences_UserId",
                table: "UserExperiences",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CvCertifications_UserCertifications_CertificationId",
                table: "CvCertifications",
                column: "CertificationId",
                principalTable: "UserCertifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CvEducations_UserEducations_EducationId",
                table: "CvEducations",
                column: "EducationId",
                principalTable: "UserEducations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CvExperiences_UserExperiences_ExperienceId",
                table: "CvExperiences",
                column: "ExperienceId",
                principalTable: "UserExperiences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CvCertifications_UserCertifications_CertificationId",
                table: "CvCertifications");

            migrationBuilder.DropForeignKey(
                name: "FK_CvEducations_UserEducations_EducationId",
                table: "CvEducations");

            migrationBuilder.DropForeignKey(
                name: "FK_CvExperiences_UserExperiences_ExperienceId",
                table: "CvExperiences");

            migrationBuilder.DropTable(
                name: "UserCertifications");

            migrationBuilder.DropTable(
                name: "UserEducations");

            migrationBuilder.DropTable(
                name: "UserExperiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CvExperiences",
                table: "CvExperiences");

            migrationBuilder.DropIndex(
                name: "IX_CvExperiences_ExperienceId",
                table: "CvExperiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CvEducations",
                table: "CvEducations");

            migrationBuilder.DropIndex(
                name: "IX_CvEducations_EducationId",
                table: "CvEducations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CvCertifications",
                table: "CvCertifications");

            migrationBuilder.RenameColumn(
                name: "ExperienceId",
                table: "CvExperiences",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "EducationId",
                table: "CvEducations",
                newName: "Town");

            migrationBuilder.RenameColumn(
                name: "CertificationId",
                table: "CvCertifications",
                newName: "Title");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CvExperiences",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "CvExperiences",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CvExperiences",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndsAt",
                table: "CvExperiences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartsAt",
                table: "CvExperiences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CvEducations",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CvEducations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "CvEducations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "CvEducations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "YearEnd",
                table: "CvEducations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "YearStart",
                table: "CvEducations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CvCertifications",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "CvCertifications",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "CvCertifications",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "CvCertifications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CvExperiences",
                table: "CvExperiences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CvEducations",
                table: "CvEducations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CvCertifications",
                table: "CvCertifications",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CvExperiences_CvId",
                table: "CvExperiences",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_CvEducations_CvId",
                table: "CvEducations",
                column: "CvId");
        }
    }
}
