using Microsoft.EntityFrameworkCore.Migrations;

namespace BSK_proj2.Data.Migrations
{
    public partial class AddedImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Access",
                table: "Images",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Comment",
                table: "Images",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Like",
                table: "Images",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LinkType",
                table: "Images",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Access",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Like",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "LinkType",
                table: "Images");
        }
    }
}
