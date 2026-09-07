using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankStatement.Demo.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToBankStatement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "bank_statements",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "bank_statements");
        }
    }
}
