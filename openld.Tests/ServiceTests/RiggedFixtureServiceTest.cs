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
    public class RiggedFixtureServiceTest : OpenLDUnitTest {
        private static RiggedFixtureService initService(OpenLDContext context) {
            return new RiggedFixtureService(
                context,
                new StructureService(
                    context,
                    new DrawingService(
                        context,
                        new TemplateService(context),
                        new ViewService(context),
                        _mapper
                    ),
                    new ViewService(context),
                    _mapper
                ),
                _mapper
            );
        }

        [Fact]
        public async Task AddRiggedFixture() {
            RiggedFixture rFixture = new RiggedFixture {
                Id = "newRFixture",
                Name = "newRFixture",
                Fixture = testFixture,
                Structure = testDrawings[0].Views[0].Structures[0],
                Position = new Point { x = 1, y = 1 },
                Angle = 0,
                Channel = 0,
                Address = 0,
                Universe = 0,
                Mode = testFixture.Modes[0],
                Notes = "",
                Colour = ""
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).AddRiggedFixtureAsync(rFixture),
                context => context.RiggedFixtures.Where(rf => rf.Id == rFixture.Id).ToList()
                    .Should().HaveCount(1)
                    .And.AllBeEquivalentTo(rFixture)
            );
        }

        [Fact]
        public async Task AddRiggedFixture_NoMode() {
            RiggedFixture rFixture = new RiggedFixture {
                Id = "newRFixture",
                Name = "newRFixture",
                Fixture = testFixture,
                Structure = testDrawings[0].Views[0].Structures[0],
                Position = new Point { x = 1, y = 1 },
                Angle = 0,
                Channel = 0,
                Address = 0,
                Universe = 0,
                Notes = "",
                Colour = ""
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).AddRiggedFixtureAsync(rFixture),
                context => context.RiggedFixtures.Where(rf => rf.Id == rFixture.Id).ToList()
                    .Should().HaveCount(1)
                    .And.AllBeEquivalentTo(
                        rFixture,
                        options => options.Excluding(rf => rf.Mode)
                    )
            );
        }

        [Fact]
        public async Task AddRiggedFixture_StructureNotExists() {
            RiggedFixture rFixture = new RiggedFixture {
                Id = "newRFixture",
                Name = "newRFixture",
                Fixture = testFixture,
                Structure = new Structure { Id = "doesn't exist" },
                Position = new Point { x = 1, y = 1 },
                Angle = 0,
                Channel = 0,
                Address = 0,
                Universe = 0,
                Mode = testFixture.Modes[0],
                Notes = "",
                Colour = ""
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).AddRiggedFixtureAsync(rFixture),
                async (act, context) => await (act.Should().ThrowAsync<KeyNotFoundException>()).WithMessage("Structure ID not found")
            );
        }

        [Fact]
        public async Task AddRiggedFixture_FixtureNotExists() {
            RiggedFixture rFixture = new RiggedFixture {
                Id = "newRFixture",
                Name = "newRFixture",
                Fixture = new Fixture { Id = "doesn't exist" },
                Structure = testDrawings[0].Views[0].Structures[0],
                Position = new Point { x = 1, y = 1 },
                Angle = 0,
                Channel = 0,
                Address = 0,
                Universe = 0,
                Mode = testFixture.Modes[0],
                Notes = "",
                Colour = ""
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).AddRiggedFixtureAsync(rFixture),
                async (act, context) => await (act.Should().ThrowAsync<KeyNotFoundException>()).WithMessage("Fixture ID not found")
            );
        }

        [Fact]
        public async Task GetDrawingAsync() {
            await _fixture.RunWithDatabaseAsync<Drawing>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetDrawingAsync(testDrawings[0].Views[0].Structures[0].Fixtures[0]),
                (result, context) => result.Should().BeEquivalentTo(
                    testDrawings[0],
                    options => options.Excluding(d => d.Owner)
                        .Excluding(d => d.Views)
                        .Excluding(
                            d => d.SelectedMemberPath.EndsWith(".Drawing")
                            || d.SelectedMemberPath.EndsWith(".Labels")
                            || d.SelectedMemberPath.EndsWith(".Structures")
                        )
                )
            );
        }

        [Fact]
        public async Task GetDrawingAsync_RFixtureNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetDrawingAsync(new RiggedFixture { Id = "doesn't exist" }),
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
                context => initService(context).GetViewAsync(testDrawings[0].Views[0].Structures[0].Fixtures[0]),
                (result, context) => result.Should().BeEquivalentTo(
                    testDrawings[0].Views[0],
                    options => options.Excluding(v => v.Drawing)
                        .Excluding(v => v.Structures)
                        .Excluding(v => v.Labels)
                )
            );
        }

        [Fact]
        public async Task GetViewAsync_RFixtureNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetViewAsync(new RiggedFixture { Id = "doesn't exist" }),
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
                context => initService(context).GetStructureAsync(testDrawings[0].Views[0].Structures[0].Fixtures[0]),
                (result, context) => result.Should().BeEquivalentTo(
                    testDrawings[0].Views[0].Structures[0],
                    options => options.IgnoringCyclicReferences()
                        .Excluding(s => s.View)
                        .Excluding(s => s.Fixtures)
                        .Excluding(s => s.Type)
                )
            );
        }

        [Fact]
        public async Task GetStructureAsync_RFixtureNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetStructureAsync(new RiggedFixture { Id = "doesn't exist" }),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task GetRiggedFixtureAsync() {
            await _fixture.RunWithDatabaseAsync<RiggedFixture>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetRiggedFixtureAsync(testDrawings[0].Views[0].Structures[0].Fixtures[0].Id),
                (result, context) => result.Should().BeEquivalentTo(
                    testDrawings[0].Views[0].Structures[0].Fixtures[0],
                    options => options.Excluding(rf => rf.Fixture)
                        .Excluding(rf => rf.Structure)
                        .Excluding(rf => rf.Mode.Fixture)
                )
            );
        }

        [Fact]
        public async Task GetRiggedFixtureAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetRiggedFixtureAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task UpdatePropsAsync() {
            RiggedFixture updated = new RiggedFixture {
                Id = testDrawings[0].Views[0].Structures[0].Fixtures[0].Id,
                Name = "Updated Name",
                Angle = 20,
                Channel = 20,
                Address = 20,
                Universe = 20,
                Notes = "Updated Notes",
                Colour = "Updated Colour"
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdatePropsAsync(updated),
                context => context.RiggedFixtures.First(rf => rf.Id == updated.Id)
                    .Should().BeEquivalentTo(
                        updated,
                        options => options.Excluding(rf => rf.Structure)
                            .Excluding(rf => rf.Position)
                            .Excluding(rf => rf.Mode)
                    )
            );
        }
        [Fact]
        public async Task UpdatePropsAsync_StructureNotUpdated() {
            RiggedFixture updated = new RiggedFixture {
                Id = testDrawings[0].Views[0].Structures[0].Fixtures[0].Id,
                Structure = testDrawings[1].Views[0].Structures[0]
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdatePropsAsync(updated),
                context => context.RiggedFixtures.Include(rf => rf.Structure).First(rf => rf.Id == updated.Id).Structure.Id
                    .Should().Be(testDrawings[0].Views[0].Structures[0].Id)
            );
        }

        [Fact]
        public async Task UpdatePropsAsync_PositionNotUpdated() {
            RiggedFixture updated = new RiggedFixture {
                Id = testDrawings[0].Views[0].Structures[0].Fixtures[0].Id,
                Position = new Point { x = 1, y = 1 }
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdatePropsAsync(updated),
                context => context.RiggedFixtures.First(rf => rf.Id == updated.Id).Position
                    .Should().BeEquivalentTo(testDrawings[0].Views[0].Structures[0].Fixtures[0].Position)
            );
        }

        [Fact]
        public async Task UpdatePropsAsync_RFixtureNotExists() {
            RiggedFixture updated = new RiggedFixture {
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
        public async Task UpdatePositionAsync() {
            RiggedFixture updated = new RiggedFixture {
                Id = testDrawings[0].Views[0].Structures[0].Fixtures[0].Id,
                Position = new Point { x = 2, y = 2 }
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdatePositionAsync(updated),
                context => context.RiggedFixtures.First(rf => rf.Id == updated.Id).Position
                    .Should().Be(updated.Position)
            );
        }

        [Fact]
        public async Task UpdatePositionAsync_RFixtureNotExists() {
            RiggedFixture updated = new RiggedFixture {
                Id = "doesn't exist",
                Position = new Point { x = 2, y = 2 }
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdatePositionAsync(updated),
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
                context => initService(context).DeleteAsync(testDrawings[0].Views[0].Structures[0].Fixtures[0].Id),
                context => context.Labels.Where(l => l.Id == testDrawings[0].Views[0].Structures[0].Fixtures[0].Id).ToList()
                    .Should().BeEmpty()
            );
        }

        [Fact]
        public async Task DeleteAsync_RFixtureNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).DeleteAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }
    }
}