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
    public class LabelServiceTest : OpenLDUnitTest {
        private static LabelService initService(OpenLDContext context) {
            return new LabelService(context, new ViewService(context), _mapper);
        }

        [Fact]
        public async Task GetLabelAsync() {
            await _fixture.RunWithDatabaseAsync<Label>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetLabelAsync(testDrawings[0].Views[0].Labels[0].Id),
                (result, context) => result.Should().BeEquivalentTo(
                    testDrawings[0].Views[0].Labels[0],
                    options => options.Excluding(l => l.View)
                )
            );
        }

        [Fact]
        public async Task GetLabelAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetLabelAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task AddLabelAsync() {
            Label label = new Label {
                Id = "newLabel",
                View = testDrawings[0].Views[0],
                Position = new Point { x = 1, y = 1 },
                Text = "newLabel Text"
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).AddLabelAsync(label),
                context => context.Labels.Where(l => l.Id == label.Id).ToList()
                    .Should().HaveCount(1)
                    .And.AllBeEquivalentTo(label)
            );
        }

        [Fact]
        public async Task AddLabelAsync_ViewNotExists() {
            Label label = new Label {
                Id = "newLabel",
                View = new View { Id = "doesn't exist" },
                Position = new Point { x = 1, y = 1 },
                Text = "newLabel Text"
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).AddLabelAsync(label),
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
                context => initService(context).GetViewAsync(testDrawings[0].Views[0].Labels[0]),
                (result, context) => result.Should().BeEquivalentTo(
                    testDrawings[0].Views[0],
                    options => options.Excluding(v => v.Drawing)
                        .Excluding(v => v.Structures)
                        .Excluding(v => v.Labels)
                )
            );
        }

        [Fact]
        public async Task GetViewAsync_LabelNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetViewAsync(new Label { Id = "doesn't exist" }),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task GetDrawingAsync() {
            await _fixture.RunWithDatabaseAsync<Drawing>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetDrawingAsync(testDrawings[0].Views[0].Labels[0]),
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
        public async Task GetDrawingAsync_LabelNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetDrawingAsync(new Label { Id = "doesn't exist" }),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task UpdatePropsAsync() {
            Label updated = new Label {
                Id = testDrawings[0].Views[0].Labels[0].Id,
                Text = "Updated label text"
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdatePropsAsync(updated),
                context => context.Labels.First(l => l.Id == updated.Id).Text
                    .Should().Be(updated.Text)
            );
        }

        [Fact]
        public async Task UpdatePropsAsync_ViewNotUpdated() {
            Label updated = new Label {
                Id = testDrawings[0].Views[0].Labels[0].Id,
                View = testDrawings[1].Views[0]
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdatePropsAsync(updated),
                context => context.Labels.Include(l => l.View).First(l => l.Id == updated.Id).View.Id
                    .Should().Be(testDrawings[0].Views[0].Id)
            );
        }

        [Fact]
        public async Task UpdatePropsAsync_PositionNotUpdated() {
            Label updated = new Label {
                Id = testDrawings[0].Views[0].Labels[0].Id,
                Position = new Point { x = 2, y = 2 }
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdatePropsAsync(updated),
                context => context.Labels.First(l => l.Id == updated.Id).Position
                    .Should().BeEquivalentTo(testDrawings[0].Views[0].Labels[0].Position)
            );
        }

        [Fact]
        public async Task UpdatePropsAsync_LabelNotExists() {
            Label updated = new Label {
                Id = "doesn't exist",
                Text = "Updated label text"
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
            Label updated = new Label {
                Id = testDrawings[0].Views[0].Labels[0].Id,
                Position = new Point { x = 2, y = 2 }
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).UpdatePositionAsync(updated),
                context => context.Labels.First(l => l.Id == updated.Id).Position
                    .Should().Be(updated.Position)
            );
        }

        [Fact]
        public async Task UpdatePositionAsync_LabelNotExists() {
            Label updated = new Label {
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
                context => initService(context).DeleteAsync(testDrawings[0].Views[0].Labels[0].Id),
                context => context.Labels.Where(l => l.Id == testDrawings[0].Views[0].Labels[0].Id).ToList()
                    .Should().BeEmpty()
            );
        }

        [Fact]
        public async Task DeleteAsync_LabelNotExists() {
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