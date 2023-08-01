using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTracker.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserThree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JwtToken",
                schema: "TimeTracker",
                table: "User",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JwtToken",
                schema: "TimeTracker",
                table: "User");
        }
    }
}
