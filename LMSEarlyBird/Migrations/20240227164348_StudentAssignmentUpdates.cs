using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMSEarlyBird.Migrations
{
    /// <inheritdoc />
    public partial class StudentAssignmentUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "StudentAssignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmissionComment",
                table: "StudentAssignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmissionTime",
                table: "StudentAssignments",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "StudentAssignments");

            migrationBuilder.DropColumn(
                name: "SubmissionComment",
                table: "StudentAssignments");

            migrationBuilder.DropColumn(
                name: "SubmissionTime",
                table: "StudentAssignments");
        }
    }
}
