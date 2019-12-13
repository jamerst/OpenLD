using Microsoft.EntityFrameworkCore.Migrations;

namespace openld.Migrations
{
    public partial class FixtureImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Fixture",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoredImages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Path = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredImages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoredImages");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Fixture");
        }
    }
}
