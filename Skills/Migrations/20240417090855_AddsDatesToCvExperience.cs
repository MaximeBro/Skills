using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skills.Migrations
{
    /// <inheritdoc />
    public partial class AddsDatesToCvExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateInfo",
                table: "CvExperiences",
                newName: "StartsAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndsAt",
                table: "CvExperiences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndsAt",
                table: "CvExperiences");

            migrationBuilder.RenameColumn(
                name: "StartsAt",
                table: "CvExperiences",
                newName: "DateInfo");
        }
    }
}
