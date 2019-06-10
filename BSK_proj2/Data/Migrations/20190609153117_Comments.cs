using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BSK_proj2.Data.Migrations
{
    public partial class Comments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    ImageID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Comments_Images_ImageID",
                        column: x => x.ImageID,
                        principalTable: "Images",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentPermissions",
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
                    table.PrimaryKey("PK_CommentPermissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CommentPermissions_Comments_ObjectID",
                        column: x => x.ObjectID,
                        principalTable: "Comments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentPermissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentPermissions_ObjectID",
                table: "CommentPermissions",
                column: "ObjectID");

            migrationBuilder.CreateIndex(
                name: "IX_CommentPermissions_UserId",
                table: "CommentPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ImageID",
                table: "Comments",
                column: "ImageID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentPermissions");

            migrationBuilder.DropTable(
                name: "Comments");
        }
    }
}
