using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTracker.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserFour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JwtToken",
                schema: "TimeTracker",
                table: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JwtToken",
                schema: "TimeTracker",
                table: "User",
                type: "text",
                nullable: true);
        }
    }
}
