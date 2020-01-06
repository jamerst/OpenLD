using Microsoft.EntityFrameworkCore.Migrations;

namespace openld.Migrations
{
    public partial class CascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RiggedFixtures_Structures_StructureId",
                table: "RiggedFixtures");

            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Views_ViewId",
                table: "Structures");

            migrationBuilder.DropForeignKey(
                name: "FK_Views_Drawings_DrawingId",
                table: "Views");

            migrationBuilder.AlterColumn<string>(
                name: "DrawingId",
                table: "Views",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ViewId",
                table: "Structures",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StructureId",
                table: "RiggedFixtures",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RiggedFixtures_Structures_StructureId",
                table: "RiggedFixtures",
                column: "StructureId",
                principalTable: "Structures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Views_ViewId",
                table: "Structures",
                column: "ViewId",
                principalTable: "Views",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Views_Drawings_DrawingId",
                table: "Views",
                column: "DrawingId",
                principalTable: "Drawings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RiggedFixtures_Structures_StructureId",
                table: "RiggedFixtures");

            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Views_ViewId",
                table: "Structures");

            migrationBuilder.DropForeignKey(
                name: "FK_Views_Drawings_DrawingId",
                table: "Views");

            migrationBuilder.AlterColumn<string>(
                name: "DrawingId",
                table: "Views",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ViewId",
                table: "Structures",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "StructureId",
                table: "RiggedFixtures",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_RiggedFixtures_Structures_StructureId",
                table: "RiggedFixtures",
                column: "StructureId",
                principalTable: "Structures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Views_ViewId",
                table: "Structures",
                column: "ViewId",
                principalTable: "Views",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Views_Drawings_DrawingId",
                table: "Views",
                column: "DrawingId",
                principalTable: "Drawings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
