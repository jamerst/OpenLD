using Microsoft.EntityFrameworkCore.Migrations;

namespace openld.Migrations
{
    public partial class CascadeModesUserDrawing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FixtureModes_Fixtures_FixtureId",
                table: "FixtureModes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDrawings_Drawings_DrawingId",
                table: "UserDrawings");

            migrationBuilder.AlterColumn<string>(
                name: "DrawingId",
                table: "UserDrawings",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FixtureId",
                table: "FixtureModes",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FixtureModes_Fixtures_FixtureId",
                table: "FixtureModes",
                column: "FixtureId",
                principalTable: "Fixtures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDrawings_Drawings_DrawingId",
                table: "UserDrawings",
                column: "DrawingId",
                principalTable: "Drawings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FixtureModes_Fixtures_FixtureId",
                table: "FixtureModes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDrawings_Drawings_DrawingId",
                table: "UserDrawings");

            migrationBuilder.AlterColumn<string>(
                name: "DrawingId",
                table: "UserDrawings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "FixtureId",
                table: "FixtureModes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_FixtureModes_Fixtures_FixtureId",
                table: "FixtureModes",
                column: "FixtureId",
                principalTable: "Fixtures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDrawings_Drawings_DrawingId",
                table: "UserDrawings",
                column: "DrawingId",
                principalTable: "Drawings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
