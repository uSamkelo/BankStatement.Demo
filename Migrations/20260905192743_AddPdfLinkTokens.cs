using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankStatement.Demo.Migrations
{
    /// <inheritdoc />
    public partial class AddPdfLinkTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pdf_link_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Signature = table.Column<string>(type: "text", nullable: false),
                    BankStatementId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pdf_link_tokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pdf_link_tokens_Signature",
                table: "pdf_link_tokens",
                column: "Signature",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pdf_link_tokens");
        }
    }
}
