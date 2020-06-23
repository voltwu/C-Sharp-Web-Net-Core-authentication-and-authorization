using Microsoft.EntityFrameworkCore.Migrations;

namespace authBasics.Migrations
{
    public partial class changeEmails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gmail",
                table: "users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Outlook",
                table: "users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QQ",
                table: "users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gmail",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Outlook",
                table: "users");

            migrationBuilder.DropColumn(
                name: "QQ",
                table: "users");
        }
    }
}
