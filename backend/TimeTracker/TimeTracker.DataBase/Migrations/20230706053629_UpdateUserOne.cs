using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTracker.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "TimeTracker",
                table: "User",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "TimeTracker",
                table: "User",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                schema: "TimeTracker",
                table: "User",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "TimeTracker",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "TimeTracker",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Username",
                schema: "TimeTracker",
                table: "User");
        }
    }
}
