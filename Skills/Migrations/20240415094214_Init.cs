using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CVs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    Job = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SafetyCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyCertifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillsTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillsTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CvCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CvId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CvInfoId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CvCertifications_CVs_CvId",
                        column: x => x.CvId,
                        principalTable: "CVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CvCertifications_CVs_CvInfoId",
                        column: x => x.CvInfoId,
                        principalTable: "CVs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CvEducations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CvId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    CvInfoId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvEducations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CvEducations_CVs_CvId",
                        column: x => x.CvId,
                        principalTable: "CVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CvEducations_CVs_CvInfoId",
                        column: x => x.CvInfoId,
                        principalTable: "CVs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CvExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CvId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    DateInfo = table.Column<string>(type: "TEXT", nullable: false),
                    CvInfoId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CvExperiences_CVs_CvId",
                        column: x => x.CvId,
                        principalTable: "CVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CvExperiences_CVs_CvInfoId",
                        column: x => x.CvInfoId,
                        principalTable: "CVs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CvSafetyCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CvId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CertId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Granted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CvInfoId = table.Column<Guid>(type: "TEXT", nullable: true)
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
                        name: "FK_CvSafetyCertifications_CVs_CvInfoId",
                        column: x => x.CvInfoId,
                        principalTable: "CVs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDisabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    GroupId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbstractSkillModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    SubCategory = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    GroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CvId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SubCategoryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CvInfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SoftSkillModel_CvInfoId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbstractSkillModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbstractSkillModel_CVs_CvId",
                        column: x => x.CvId,
                        principalTable: "CVs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AbstractSkillModel_CVs_CvInfoId",
                        column: x => x.CvInfoId,
                        principalTable: "CVs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AbstractSkillModel_CVs_SoftSkillModel_CvInfoId",
                        column: x => x.SoftSkillModel_CvInfoId,
                        principalTable: "CVs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AbstractSkillModel_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AbstractSkillModel_SkillsTypes_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "SkillsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbstractSkillModel_SkillsTypes_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SkillsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbstractSkillModel_SkillsTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SkillsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TypesLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypesLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TypesLevels_SkillsTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SkillsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoftTypesLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SkillId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftTypesLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoftTypesLevels_AbstractSkillModel_SkillId",
                        column: x => x.SkillId,
                        principalTable: "AbstractSkillModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersSkills",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SkillId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsSoftSkill = table.Column<bool>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersSkills", x => new { x.UserId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_UsersSkills_AbstractSkillModel_SkillId",
                        column: x => x.SkillId,
                        principalTable: "AbstractSkillModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersSkills_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbstractSkillModel_CategoryId",
                table: "AbstractSkillModel",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AbstractSkillModel_CvId",
                table: "AbstractSkillModel",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_AbstractSkillModel_CvInfoId",
                table: "AbstractSkillModel",
                column: "CvInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_AbstractSkillModel_GroupId",
                table: "AbstractSkillModel",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AbstractSkillModel_SoftSkillModel_CvInfoId",
                table: "AbstractSkillModel",
                column: "SoftSkillModel_CvInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_AbstractSkillModel_SubCategoryId",
                table: "AbstractSkillModel",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AbstractSkillModel_TypeId",
                table: "AbstractSkillModel",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CvCertifications_CvId",
                table: "CvCertifications",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_CvCertifications_CvInfoId",
                table: "CvCertifications",
                column: "CvInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_CvEducations_CvId",
                table: "CvEducations",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_CvEducations_CvInfoId",
                table: "CvEducations",
                column: "CvInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_CvExperiences_CvId",
                table: "CvExperiences",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_CvExperiences_CvInfoId",
                table: "CvExperiences",
                column: "CvInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_CvSafetyCertifications_CvId",
                table: "CvSafetyCertifications",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_CvSafetyCertifications_CvInfoId",
                table: "CvSafetyCertifications",
                column: "CvInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_SoftTypesLevels_SkillId",
                table: "SoftTypesLevels",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_TypesLevels_TypeId",
                table: "TypesLevels",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GroupId",
                table: "Users",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersSkills_SkillId",
                table: "UsersSkills",
                column: "SkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CvCertifications");

            migrationBuilder.DropTable(
                name: "CvEducations");

            migrationBuilder.DropTable(
                name: "CvExperiences");

            migrationBuilder.DropTable(
                name: "CvSafetyCertifications");

            migrationBuilder.DropTable(
                name: "SafetyCertifications");

            migrationBuilder.DropTable(
                name: "SoftTypesLevels");

            migrationBuilder.DropTable(
                name: "TypesLevels");

            migrationBuilder.DropTable(
                name: "UsersSkills");

            migrationBuilder.DropTable(
                name: "AbstractSkillModel");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CVs");

            migrationBuilder.DropTable(
                name: "SkillsTypes");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
