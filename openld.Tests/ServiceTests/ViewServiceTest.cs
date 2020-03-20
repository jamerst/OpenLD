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
    public class ViewServiceTest : OpenLDUnitTest {
        private static ViewService initService(OpenLDContext context) {
            return new ViewService(context);
        }

        [Fact]
        public async Task GetDrawingAsync() {
            await _fixture.RunWithDatabaseAsync<Drawing>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetDrawingAsync(testDrawings[0].Views[0]),
                (result, context) => result.Should().BeEquivalentTo(
                    testDrawings[0],
                    options => options.IgnoringCyclicReferences()
                        .Excluding(d => d.Owner)
                        .Excluding(d => d.Views)
                )
            );
        }

        [Fact]
        public async Task GetDrawingAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetDrawingAsync(new View { Id = "doesn't exist" }),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task CreateViewAsync() {
            View newView = new View {
                Drawing = testDrawings[0],
                Structures = new List<Structure>(),
                Labels = new List<Label>(),
                Name = "newView",
                Width = 25,
                Height = 25,
                Type = 0
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).CreateViewAsync(newView),
                context => context.Views.Where(v => v.Name == newView.Name).ToList()
                    .Should().HaveCount(1)
                    .And.AllBeEquivalentTo(
                        newView,
                        options => options.IgnoringCyclicReferences()
                            .Excluding(v => v.Id)
                            .Excluding(
                                v => v.SelectedMemberPath.Contains(".Drawing")
                                && !v.SelectedMemberPath.EndsWith(".Drawing.Id")
                            )
                    )
            );
        }

        [Fact]
        public async Task CreateViewAsync_DrawingNotExists() {
            View newView = new View {
                Drawing = new Drawing { Id = "doesn't exist" },
                Structures = new List<Structure>(),
                Labels = new List<Label>(),
                Name = "newView",
                Width = 25,
                Height = 25,
                Type = 0
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).CreateViewAsync(newView),
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
                context => initService(context).GetViewAsync(testDrawings[0].Views[0].Id),
                (result, context) => result.Should().BeEquivalentTo(
                    testDrawings[0].Views[0],
                    options => options.Excluding(v => v.Drawing)
                        .Excluding(v => v.Structures)
                        .Excluding(v => v.Labels)
                )
            );
        }

        [Fact]
        public async Task GetViewAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetViewAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task DeleteViewAsync() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).DeleteViewAsync(testDrawings[0].Views[0].Id),
                context => context.Views.Where(v => v.Id == testDrawings[0].Views[0].Id).ToList()
                    .Should().BeEmpty()
            );
        }

        [Fact]
        public async Task DeleteViewAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).DeleteViewAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task DeleteViewAsync_LastViewInDrawing() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).DeleteViewAsync(testDrawings[1].Views[0].Id),
                async (act, context) => await act.Should().ThrowAsync<InvalidOperationException>()
            );
        }

        [Fact]
        public async Task UpdateLastModifiedAsync() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdateLastModifiedAsync(testDrawings[0].Views[0]),
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
                context => initService(context).UpdateLastModifiedAsync(new View { Id = "doesn't exist" }),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task GetUsedFixturesAsync() {
            var expected = new Tuple<List<UsedFixtureResult>, List<RiggedFixture>>(
                new List<UsedFixtureResult> {
                    new UsedFixtureResult {
                        Fixture = testFixture,
                        Count = 2
                    }
                },
                new List<RiggedFixture>(testDrawings[0].Views[0].Structures[0].Fixtures)
            );

            await _fixture.RunWithDatabaseAsync<Tuple<List<UsedFixtureResult>, List<RiggedFixture>>>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetUsedFixturesAsync(testDrawings[0].Views[0].Id),
                (result, context) => result.Should().BeEquivalentTo(
                    expected,
                    options => options.IgnoringCyclicReferences()
                        .Excluding(d => d.SelectedMemberPath.EndsWith(".Fixture.Modes"))
                        .Excluding(d => d.SelectedMemberPath.EndsWith(".Mode.Fixture"))
                        .Excluding(d => d.SelectedMemberPath.Contains(".Type"))
                        .Excluding(d => d.SelectedMemberPath.Contains(".Structure.View."))
                )
            );
        }

        [Fact]
        public async Task GetUsedFixturesAsync_NoUsedFixtures() {
            var expected = new Tuple<List<UsedFixtureResult>, List<RiggedFixture>>(
                new List<UsedFixtureResult>(),
                new List<RiggedFixture>()
            );

            await _fixture.RunWithDatabaseAsync<Tuple<List<UsedFixtureResult>, List<RiggedFixture>>>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetUsedFixturesAsync(testDrawings[0].Views[1].Id),
                (result, context) => result.Should().BeEquivalentTo(
                    expected,
                    options => options.IgnoringCyclicReferences()
                        .Excluding(d => d.SelectedMemberPath.EndsWith(".Fixture.Modes"))
                        .Excluding(d => d.SelectedMemberPath.EndsWith(".Mode.Fixture"))
                        .Excluding(d => d.SelectedMemberPath.Contains(".Type"))
                        .Excluding(d => d.SelectedMemberPath.Contains(".Structure.View."))
                )
            );
        }

        [Fact]
        public async Task GetUsedFixturesAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetUsedFixturesAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }
    }
}