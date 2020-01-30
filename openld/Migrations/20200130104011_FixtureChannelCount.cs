using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace openld.Migrations
{
    public partial class FixtureChannelCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Channels",
                table: "FixtureModes",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "Channels",
                table: "FixtureModes",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
