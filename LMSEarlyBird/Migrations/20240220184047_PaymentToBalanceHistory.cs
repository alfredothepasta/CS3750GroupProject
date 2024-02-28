using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMSEarlyBird.Migrations
{
    /// <inheritdoc />
    public partial class PaymentToBalanceHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentTime",
                table: "PaymentHistory",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "PaymentHistory",
                newName: "NetChange");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "PaymentHistory",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "PaymentHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "PaymentHistory");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "PaymentHistory");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "PaymentHistory",
                newName: "PaymentTime");

            migrationBuilder.RenameColumn(
                name: "NetChange",
                table: "PaymentHistory",
                newName: "Amount");
        }
    }
}
