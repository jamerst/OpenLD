using Microsoft.EntityFrameworkCore.Migrations;

namespace openld.Migrations
{
    public partial class RiggedFixtureChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "Angle",
                table: "RiggedFixtures",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<short>(
                name: "Channel",
                table: "RiggedFixtures",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channel",
                table: "RiggedFixtures");

            migrationBuilder.AlterColumn<int>(
                name: "Angle",
                table: "RiggedFixtures",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short));
        }
    }
}
