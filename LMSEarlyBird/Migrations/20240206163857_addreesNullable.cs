using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMSEarlyBird.Migrations
{
    /// <inheritdoc />
    public partial class addreesNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_instructorCourses_AspNetUsers_UserId",
                table: "instructorCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_instructorCourses_Courses_CourseId",
                table: "instructorCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_instructorCourses",
                table: "instructorCourses");

            migrationBuilder.RenameTable(
                name: "instructorCourses",
                newName: "InstructorCourses");

            migrationBuilder.RenameIndex(
                name: "IX_instructorCourses_CourseId",
                table: "InstructorCourses",
                newName: "IX_InstructorCourses_CourseId");

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LineTwo",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LineOne",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstructorCourses",
                table: "InstructorCourses",
                columns: new[] { "UserId", "CourseId" });

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorCourses_AspNetUsers_UserId",
                table: "InstructorCourses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorCourses_Courses_CourseId",
                table: "InstructorCourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstructorCourses_AspNetUsers_UserId",
                table: "InstructorCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_InstructorCourses_Courses_CourseId",
                table: "InstructorCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InstructorCourses",
                table: "InstructorCourses");

            migrationBuilder.RenameTable(
                name: "InstructorCourses",
                newName: "instructorCourses");

            migrationBuilder.RenameIndex(
                name: "IX_InstructorCourses_CourseId",
                table: "instructorCourses",
                newName: "IX_instructorCourses_CourseId");

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LineTwo",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LineOne",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_instructorCourses",
                table: "instructorCourses",
                columns: new[] { "UserId", "CourseId" });

            migrationBuilder.AddForeignKey(
                name: "FK_instructorCourses_AspNetUsers_UserId",
                table: "instructorCourses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_instructorCourses_Courses_CourseId",
                table: "instructorCourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
