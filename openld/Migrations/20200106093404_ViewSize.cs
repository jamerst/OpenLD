using Microsoft.EntityFrameworkCore.Migrations;

namespace openld.Migrations
{
    public partial class ViewSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "Drawings");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Drawings");

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "Views",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "Views",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "Views");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Views");

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "Drawings",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Width",
                table: "Drawings",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
