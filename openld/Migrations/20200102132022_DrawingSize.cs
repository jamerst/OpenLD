using Microsoft.EntityFrameworkCore.Migrations;

namespace openld.Migrations
{
    public partial class DrawingSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "Drawings",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "Drawings",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "Drawings");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Drawings");
        }
    }
}
