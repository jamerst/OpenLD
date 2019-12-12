using Microsoft.EntityFrameworkCore.Migrations;

namespace openld.Migrations
{
    public partial class StoredImageInFixture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Fixture");

            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "Fixture",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fixture_ImageId",
                table: "Fixture",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fixture_StoredImages_ImageId",
                table: "Fixture",
                column: "ImageId",
                principalTable: "StoredImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fixture_StoredImages_ImageId",
                table: "Fixture");

            migrationBuilder.DropIndex(
                name: "IX_Fixture_ImageId",
                table: "Fixture");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Fixture");

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Fixture",
                type: "text",
                nullable: true);
        }
    }
}
