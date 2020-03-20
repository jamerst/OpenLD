using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

using openld.Data;
using openld.Models;
using openld.Services;

namespace openld.Tests {
    public class StructureServiceTest : OpenLDUnitTest {
        private static StructureService initService(OpenLDContext context) {
            return new StructureService(
                context,
                new DrawingService(
                    context,
                    new TemplateService(context),
                    new ViewService(context),
                    _mapper
                ),
                new ViewService(context),
                _mapper
            );
        }

        [Fact]
        public async Task GetDrawingAsync() {
            await _fixture.RunWithDatabaseAsync<Drawing>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetDrawingAsync(testDrawings[0].Views[0].Structures[0]),
                (result, context) => result.Should().BeEquivalentTo(
                    testDrawings[0],
                    options => options.Excluding(d => d.Owner)
                        .Excluding(
                            d => d.SelectedMemberPath.EndsWith(".Drawing")
                            || d.SelectedMemberPath.EndsWith(".Labels")
                            || d.SelectedMemberPath.EndsWith(".Structures")
                        )
                )
            );
        }

        [Fact]
        public async Task GetDrawingAsync_StructureNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetDrawingAsync(new Structure { Id = "doesn't exist" }),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task GetViewAsync() {
            await _fixture.RunWithDatabaseAsync<View>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetViewAsync(testDrawings[0].Views[0].Structures[0]),
                (result, context) => result.Should().BeEquivalentTo(
                    testDrawings[0].Views[0],
                    options => options.Excluding(v => v.Drawing)
                        .Excluding(v => v.Structures)
                        .Excluding(v => v.Labels)
                )
            );
        }

        [Fact]
        public async Task GetViewAsync_StructureNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetViewAsync(new Structure { Id = "doesn't exist" }),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task GetStructureAsync() {
            await _fixture.RunWithDatabaseAsync<Structure>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetStructureAsync(testDrawings[0].Views[0].Structures[0].Id),
                (result, context) => result.Should().BeEquivalentTo(
                    testDrawings[0].Views[0].Structures[0],
                    options => options.IgnoringCyclicReferences()
                        .Excluding(s => s.View)
                        .Excluding(s => s.Fixtures)
                )
            );
        }

        [Fact]
        public async Task GetStructureAsync_StructureNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetStructureAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task AddStructureAsync() {
            Structure structure = new Structure {
                Id = "newStructure",
                View = testDrawings[0].Views[0],
                Geometry = new Geometry {
                    Points = new List<Point> {
                        new Point { x = 3, y = 3 },
                        new Point { x = 4, y = 4 }
                    }
                },
                Name = "newStructure",
                Rating = 50,
                Type = testStructureTypes[0],
                Notes = ""
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    context.StructureTypes.AddRange(testStructureTypes);
                    await context.SaveChangesAsync();
                },
                context => initService(context).AddStructureAsync(structure),
                context => context.Structures.Where(s => s.Id == structure.Id).ToList()
                    .Should().HaveCount(1)
                    .And.AllBeEquivalentTo(structure)
            );
        }

        [Fact]
        public async Task AddStructureAsync_ViewNotExists() {
            Structure structure = new Structure {
                Id = "newStructure",
                View = new View { Id = "doesn't exist" },
                Geometry = new Geometry {
                    Points = new List<Point> {
                        new Point { x = 3, y = 3 },
                        new Point { x = 4, y = 4 }
                    }
                },
                Name = "newStructure",
                Rating = 50,
                Type = testStructureTypes[0],
                Notes = ""
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    context.StructureTypes.AddRange(testStructureTypes);
                    await context.SaveChangesAsync();
                },
                context => initService(context).AddStructureAsync(structure),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task AddStructureAsync_NoType() {
            Structure structure = new Structure {
                Id = "newStructure",
                View = testDrawings[0].Views[0],
                Geometry = new Geometry {
                    Points = new List<Point> {
                        new Point { x = 3, y = 3 },
                        new Point { x = 4, y = 4 }
                    }
                },
                Name = "newStructure",
                Rating = 50,
                Notes = ""
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    context.StructureTypes.AddRange(testStructureTypes);
                    await context.SaveChangesAsync();
                },
                context => initService(context).AddStructureAsync(structure),
                context => {
                    context.Structures.Include(s => s.View).Where(s => s.Id == structure.Id).ToList()
                        .Should().HaveCount(1)
                        .And.AllBeEquivalentTo(
                            structure,
                            options => options.Excluding(s => s.Type)
                        );

                     context.Structures.Include(s => s.View).First(s => s.Id == structure.Id).Type
                        .Should().NotBeNull()
                        .And.BeEquivalentTo(testStructureTypes[2]);
                }
            );
        }

        [Fact]
        public async Task AddStructureAsync_WithFixtures() {
            Structure structure = new Structure {
                Id = "newStructure",
                View = testDrawings[0].Views[0],
                Geometry = new Geometry {
                    Points = new List<Point> {
                        new Point { x = 3, y = 3 },
                        new Point { x = 4, y = 4 }
                    }
                },
                Fixtures = new List<RiggedFixture> {
                    new RiggedFixture {
                        Id = "newStructure_Fixture1",
                        Fixture = testFixture,
                        Mode = testFixture.Modes[0]
                    },
                    new RiggedFixture {
                        Id = "newStructure_Fixture2",
                        Fixture = testFixture,
                        Mode = testFixture.Modes[1]
                    }
                },
                Name = "newStructure",
                Rating = 50,
                Type = testStructureTypes[0],
                Notes = ""
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    context.StructureTypes.AddRange(testStructureTypes);
                    await context.SaveChangesAsync();
                },
                context => initService(context).AddStructureAsync(structure),
                context => context.Structures.Where(s => s.Id == structure.Id).ToList()
                    .Should().HaveCount(1)
                    .And.AllBeEquivalentTo(structure)
            );
        }

        [Fact]
        public async Task SetStructureGeometry() {
            Geometry newGeometry = new Geometry {
                Points = new List<Point> {
                    new Point { x = 5, y = 5 },
                    new Point { x = 6, y = 6 }
                }
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).SetStructureGeometryAsync(
                    testDrawings[0].Views[0].Structures[0].Id,
                    newGeometry
                ),
                context => context.Structures.First(s => s.Id == testDrawings[0].Views[0].Structures[0].Id).Geometry
                    .Should().BeEquivalentTo(newGeometry)
            );
        }

        [Fact]
        public async Task SetStructureGeometry_StructureNotExists() {
            Geometry newGeometry = new Geometry {
                Points = new List<Point> {
                    new Point { x = 5, y = 5 },
                    new Point { x = 6, y = 6 }
                }
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).SetStructureGeometryAsync(
                    "doesn't exist",
                    newGeometry
                ),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task SetRiggedFixturePositionsAsync() {
            List<Point> newPoints = new List<Point> {
                new Point { x = 5.5, y = 5.5 },
                new Point { x = 5, y = 5 }
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).SetRiggedFixturePositionsAsync(
                    testDrawings[0].Views[0].Structures[0].Id,
                    newPoints
                ),
                context => context.RiggedFixtures
                    .Where(rf => rf.Structure.Id == testDrawings[0].Views[0].Structures[0].Id)
                    .Select(rf => rf.Position).ToList()
                    .Should().BeEquivalentTo(newPoints)
            );
        }

        [Fact]
        public async Task SetRiggedFixturePositionsAsync_StructureNotExists() {
            List<Point> newPoints = new List<Point> {
                new Point { x = 5.5, y = 5.5 },
                new Point { x = 5, y = 5 }
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).SetRiggedFixturePositionsAsync(
                    "doesn't exist",
                    newPoints
                ),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task UpdatePropsAsync() {
            Structure updated = new Structure {
                Id = testDrawings[0].Views[0].Structures[0].Id,
                Name = "Updated Name",
                Rating = 1,
                Notes = "Updated Notes"
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdatePropsAsync(updated),
                context => context.Structures.First(s => s.Id == updated.Id)
                    .Should().BeEquivalentTo(
                        updated,
                        options => options.Excluding(s => s.View)
                            .Excluding(s => s.Geometry)
                            .Excluding(s => s.Type)
                    )
            );
        }

        [Fact]
        public async Task UpdatePropsAsync_ViewNotUpdated() {
            Structure updated = new Structure {
                Id = testDrawings[0].Views[0].Structures[0].Id,
                View = testDrawings[1].Views[0]
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdatePropsAsync(updated),
                context => context.Structures.Include(s => s.View).First(s => s.Id == updated.Id).View.Id
                    .Should().Be(testDrawings[0].Views[0].Id)
            );
        }

        [Fact]
        public async Task UpdatePropsAsync_GeometryNotUpdated() {
            Structure updated = new Structure {
                Id = testDrawings[0].Views[0].Structures[0].Id,
                Geometry = new Geometry {
                    Points = new List<Point> {
                        new Point { x = 5, y = 5 },
                        new Point { x = 6, y = 6 }
                    }
                }
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdatePropsAsync(updated),
                context => context.Structures.First(s => s.Id == updated.Id).Geometry
                    .Should().BeEquivalentTo(testDrawings[0].Views[0].Structures[0].Geometry)
            );
        }

        [Fact]
        public async Task UpdatePropsAsync_RFixtureNotExists() {
            Structure updated = new Structure {
                Id = "doesn't exist",
                Name = "Updated Name"
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdatePropsAsync(updated),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task DeleteAsync() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).DeleteAsync(testDrawings[0].Views[0].Structures[0].Id),
                context => context.Labels.Where(l => l.Id == testDrawings[0].Views[0].Structures[0].Id).ToList()
                    .Should().BeEmpty()
            );
        }

        [Fact]
        public async Task DeleteAsync_StructureNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).DeleteAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task CreateTypesAsync() {
            string[] types = { "newType1", "newType2", "newType3" };

            await _fixture.RunWithDatabaseAsync(
                null,
                context => initService(context).CreateTypesAsync(types),
                context => context.StructureTypes.Select(st => st.Name).ToList()
                    .Should().BeEquivalentTo(types)
            );
        }

        [Fact]
        public async Task GetAllTypesAsync() {
            await _fixture.RunWithDatabaseAsync<List<StructureType>>(
                async context => {
                    context.StructureTypes.AddRange(testStructureTypes);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetAllTypesAsync(),
                (result, context) => result.Should().BeEquivalentTo(testStructureTypes)
            );
        }

        [Fact]
        public async Task GetStructureTypeAsync() {
            await _fixture.RunWithDatabaseAsync<StructureType>(
                async context => {
                    context.StructureTypes.AddRange(testStructureTypes);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetStructureTypeAsync(testStructureTypes[1].Id),
                (result, context) => result.Should().BeEquivalentTo(testStructureTypes[1])
            );
        }

        [Fact]
        public async Task GetStructureTypeAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.StructureTypes.AddRange(testStructureTypes);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetStructureTypeAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task GetStructureTypeByNameAsync() {
            await _fixture.RunWithDatabaseAsync<StructureType>(
                async context => {
                    context.StructureTypes.AddRange(testStructureTypes);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetStructureTypeByNameAsync("Bar"),
                (result, context) => result.Should().BeEquivalentTo(testStructureTypes[2])
            );
        }

        [Fact]
        public async Task GetStructureTypeByNameAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.StructureTypes.AddRange(testStructureTypes);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetStructureTypeByNameAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task UpdateLastModifiedAsync() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdateLastModifiedAsync(testDrawings[0].Views[0].Structures[0]),
                context => context.Drawings.First(d => d.Id == testDrawings[0].Id).LastModified
                    .Should().BeCloseTo(DateTime.Now)
            );
        }

        [Fact]
        public async Task UpdateLastModifiedAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdateLastModifiedAsync(new Structure { Id = "doesn't exist" }),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }
    }
}