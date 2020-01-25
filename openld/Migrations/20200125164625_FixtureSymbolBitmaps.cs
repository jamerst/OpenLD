using Microsoft.EntityFrameworkCore.Migrations;

namespace openld.Migrations
{
    public partial class FixtureSymbolBitmaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "Fixtures");

            migrationBuilder.AddColumn<string>(
                name: "SymbolId",
                table: "Fixtures",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Symbols",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Hash = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    BitmapId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Symbols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Symbols_StoredImages_BitmapId",
                        column: x => x.BitmapId,
                        principalTable: "StoredImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_SymbolId",
                table: "Fixtures",
                column: "SymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_Symbols_BitmapId",
                table: "Symbols",
                column: "BitmapId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fixtures_Symbols_SymbolId",
                table: "Fixtures",
                column: "SymbolId",
                principalTable: "Symbols",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fixtures_Symbols_SymbolId",
                table: "Fixtures");

            migrationBuilder.DropTable(
                name: "Symbols");

            migrationBuilder.DropIndex(
                name: "IX_Fixtures_SymbolId",
                table: "Fixtures");

            migrationBuilder.DropColumn(
                name: "SymbolId",
                table: "Fixtures");

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "Fixtures",
                type: "xml",
                nullable: true);
        }
    }
}
