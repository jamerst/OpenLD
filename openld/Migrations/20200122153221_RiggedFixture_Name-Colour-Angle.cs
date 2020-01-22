using Microsoft.EntityFrameworkCore.Migrations;

namespace openld.Migrations
{
    public partial class RiggedFixture_NameColourAngle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Angle",
                table: "RiggedFixtures",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Colour",
                table: "RiggedFixtures",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RiggedFixtures",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Angle",
                table: "RiggedFixtures");

            migrationBuilder.DropColumn(
                name: "Colour",
                table: "RiggedFixtures");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "RiggedFixtures");
        }
    }
}
