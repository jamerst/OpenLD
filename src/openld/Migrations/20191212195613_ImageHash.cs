using Microsoft.EntityFrameworkCore.Migrations;

namespace openld.Migrations
{
    public partial class ImageHash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "StoredImages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "StoredImages");
        }
    }
}
