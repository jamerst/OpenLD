using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace openld.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FixtureType",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixtureType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StructureType",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StructureType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fixture",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Manufacturer = table.Column<string>(nullable: true),
                    ReleaseData = table.Column<DateTime>(nullable: false),
                    TypeId = table.Column<string>(nullable: true),
                    Power = table.Column<int>(nullable: false),
                    Weight = table.Column<float>(nullable: false),
                    Symbol = table.Column<string>(type: "xml", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fixture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fixture_FixtureType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "FixtureType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Drawing",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    OwnerId = table.Column<string>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drawing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drawing_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FixtureMode",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    FixtureId = table.Column<string>(nullable: true),
                    Addresses = table.Column<string[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixtureMode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FixtureMode_Fixture_FixtureId",
                        column: x => x.FixtureId,
                        principalTable: "Fixture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserDrawings",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    DrawingId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDrawings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDrawings_Drawing_DrawingId",
                        column: x => x.DrawingId,
                        principalTable: "Drawing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserDrawings_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "View",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DrawingId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View", x => x.Id);
                    table.ForeignKey(
                        name: "FK_View_Drawing_DrawingId",
                        column: x => x.DrawingId,
                        principalTable: "Drawing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Structure",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ViewId = table.Column<string>(nullable: true),
                    Geo = table.Column<Geometry>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    rating = table.Column<float>(nullable: false),
                    TypeId = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Structure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Structure_StructureType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "StructureType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Structure_View_ViewId",
                        column: x => x.ViewId,
                        principalTable: "View",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RiggedFixture",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    FixtureId = table.Column<string>(nullable: true),
                    StructureId = table.Column<string>(nullable: true),
                    Position = table.Column<Point>(nullable: true),
                    Address = table.Column<short>(nullable: false),
                    Universe = table.Column<short>(nullable: false),
                    ModeId = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiggedFixture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiggedFixture_Fixture_FixtureId",
                        column: x => x.FixtureId,
                        principalTable: "Fixture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RiggedFixture_FixtureMode_ModeId",
                        column: x => x.ModeId,
                        principalTable: "FixtureMode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RiggedFixture_Structure_StructureId",
                        column: x => x.StructureId,
                        principalTable: "Structure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drawing_OwnerId",
                table: "Drawing",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixture_TypeId",
                table: "Fixture",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FixtureMode_FixtureId",
                table: "FixtureMode",
                column: "FixtureId");

            migrationBuilder.CreateIndex(
                name: "IX_RiggedFixture_FixtureId",
                table: "RiggedFixture",
                column: "FixtureId");

            migrationBuilder.CreateIndex(
                name: "IX_RiggedFixture_ModeId",
                table: "RiggedFixture",
                column: "ModeId");

            migrationBuilder.CreateIndex(
                name: "IX_RiggedFixture_StructureId",
                table: "RiggedFixture",
                column: "StructureId");

            migrationBuilder.CreateIndex(
                name: "IX_Structure_TypeId",
                table: "Structure",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Structure_ViewId",
                table: "Structure",
                column: "ViewId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDrawings_DrawingId",
                table: "UserDrawings",
                column: "DrawingId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDrawings_UserId",
                table: "UserDrawings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_View_DrawingId",
                table: "View",
                column: "DrawingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RiggedFixture");

            migrationBuilder.DropTable(
                name: "UserDrawings");

            migrationBuilder.DropTable(
                name: "FixtureMode");

            migrationBuilder.DropTable(
                name: "Structure");

            migrationBuilder.DropTable(
                name: "Fixture");

            migrationBuilder.DropTable(
                name: "StructureType");

            migrationBuilder.DropTable(
                name: "View");

            migrationBuilder.DropTable(
                name: "FixtureType");

            migrationBuilder.DropTable(
                name: "Drawing");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
