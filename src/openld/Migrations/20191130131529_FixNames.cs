using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace openld.Migrations
{
    public partial class FixNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Geo",
                table: "Structure");

            migrationBuilder.DropColumn(
                name: "ReleaseData",
                table: "Fixture");

            migrationBuilder.RenameColumn(
                name: "rating",
                table: "Structure",
                newName: "Rating");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AddColumn<Geometry>(
                name: "Geometry",
                table: "Structure",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Fixture",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Geometry",
                table: "Structure");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Fixture");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "Structure",
                newName: "rating");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AddColumn<Geometry>(
                name: "Geo",
                table: "Structure",
                type: "geometry",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseData",
                table: "Fixture",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
