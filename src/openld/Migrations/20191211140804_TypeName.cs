using Microsoft.EntityFrameworkCore.Migrations;

namespace openld.Migrations
{
    public partial class TypeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FixtureType",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "FixtureType");
        }
    }
}
