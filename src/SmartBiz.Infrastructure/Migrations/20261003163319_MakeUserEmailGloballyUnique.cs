using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartBiz.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserEmailGloballyUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_BusinessId_Email",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BusinessId_Email",
                table: "Users",
                columns: new[] { "BusinessId", "Email" },
                unique: true);
        }
    }
}
