using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace openld.Migrations
{
    public partial class FixtureModeChannels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Addresses",
                table: "FixtureModes");

            migrationBuilder.AddColumn<string[]>(
                name: "Channels",
                table: "FixtureModes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channels",
                table: "FixtureModes");

            migrationBuilder.AddColumn<string[]>(
                name: "Addresses",
                table: "FixtureModes",
                type: "text[]",
                nullable: true);
        }
    }
}
