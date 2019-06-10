using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BSK_proj2.Data.Migrations
{
    public partial class Permissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_AspNetUsers_UserId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_UserId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Images");

            migrationBuilder.CreateTable(
                name: "ImagePermissions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    read = table.Column<bool>(nullable: false),
                    write = table.Column<bool>(nullable: false),
                    delete = table.Column<bool>(nullable: false),
                    give = table.Column<bool>(nullable: false),
                    take = table.Column<bool>(nullable: false),
                    owner = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ObjectID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagePermissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ImagePermissions_Images_ObjectID",
                        column: x => x.ObjectID,
                        principalTable: "Images",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImagePermissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImagePermissions_ObjectID",
                table: "ImagePermissions",
                column: "ObjectID");

            migrationBuilder.CreateIndex(
                name: "IX_ImagePermissions_UserId",
                table: "ImagePermissions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagePermissions");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Images",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_UserId",
                table: "Images",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_AspNetUsers_UserId",
                table: "Images",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
