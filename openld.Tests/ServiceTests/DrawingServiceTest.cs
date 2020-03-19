using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

using openld.Models;
using openld.Services;

namespace openld.Tests {
    public class DrawingServiceTest : OpenLDUnitTest {

        private static Template testTemplate = new Template {
            Id = "testDrawing1_Template",
            Drawing = testDrawings[0]
        };

        [Fact]
        public async Task CreateDrawingAsync() {
            Drawing newDrawing = new Drawing {
                Id = "newDrawing1",
                Title = "newDrawing1",
                Views = new List<View> {
                    new View {Width = 10, Height = 10}
                }
            };

            await _fixture.RunWithDatabaseAsync<string>(
                async context => {
                    context.Users.AddRange(testUsers);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper).CreateDrawingAsync("user1", newDrawing),
                (result, context) => context.Drawings.Where(d => d.Id == newDrawing.Id).ToList()
                    .Should().HaveCount(1)
                    .And.BeEquivalentTo(newDrawing)
            );
        }

        [Fact]
        public async Task CreateDrawingAsync_InvalidUser() {
            Drawing newDrawing = new Drawing {
                Id = "newDrawing1",
                Title = "newDrawing1",
                Views = new List<View> {
                    new View {Width = 10, Height = 10}
                }
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .CreateDrawingAsync("doesn't exist", newDrawing),

                async (act, context) => await act.Should().ThrowAsync<UnauthorizedAccessException>()
            );
        }

        [Fact]
        public async Task CreateDrawingFromTemplate() {
            Drawing newDrawing = new Drawing {
                Id = "newDrawing1",
                Title = "newDrawing1"
            };

            await _fixture.RunWithDatabaseAsync<string>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    context.Templates.Add(testTemplate);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .CreateDrawingFromTemplateAsync(testUsers[1].Id, newDrawing, testTemplate),

                (result, context) => {
                    var created = context.Drawings
                        .Include(d => d.Views)
                            .ThenInclude(v => v.Structures)
                                .ThenInclude(s => s.Fixtures)
                                    .ThenInclude(rf => rf.Fixture)
                                        .ThenInclude(f => f.Type)
                        .First(d => d.Id == result);

                    // compare ignoring id fields, title and last modified
                    // don't ignore any field under Fixture
                    created.Should().NotBeNull()
                    .And.BeEquivalentTo(
                        testDrawings[0],
                        options => options
                            .Excluding(d => d.SelectedMemberPath.EndsWith("Id") && !d.SelectedMemberPath.Contains(".Fixture."))
                            .Excluding(d => d.Title)
                            .Excluding(d => d.LastModified)
                            .Excluding(d => d.Owner)
                            .Excluding(d => d.UserDrawings)
                            .IgnoringCyclicReferences()
                    );

                    // check owner assigned correctly
                    created.Owner.Id.Should().Be(testUsers[1].Id);
                }
            );
        }

        [Fact]
        public async Task CreateDrawingFromTemplateAsync_InvalidUser() {
            Drawing newDrawing = new Drawing {
                Id = "newDrawing1",
                Title = "newDrawing1"
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    context.Templates.Add(testTemplate);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .CreateDrawingFromTemplateAsync("doesn't exist", newDrawing, testTemplate),

                async (act, context) => await act.Should().ThrowAsync<UnauthorizedAccessException>()
            );
        }

        [Fact]
        public async Task CreateDrawingFromTemplateAsync_TemplateNotExists() {
            Drawing newDrawing = new Drawing {
                Id = "newDrawing1",
                Title = "newDrawing1"
            };

            Template template = new Template {
                Id = "doesn't exist"
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    context.Templates.Add(testTemplate);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .CreateDrawingFromTemplateAsync(testUsers[0].Id, newDrawing, template),

                async (act, context) => await (act.Should().ThrowAsync<KeyNotFoundException>()).WithMessage("Template ID not found")
            );
        }

        [Fact]
        public async Task GetDrawingAsync() {
            await _fixture.RunWithDatabaseAsync<Drawing>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .GetDrawingAsync(testDrawings[0].Id),
                (result, context) => result.Should().BeEquivalentTo(
                    testDrawings[0],
                    options => options.Excluding(d => d.Owner)
                        .Excluding(d => d.SelectedMemberPath.EndsWith(".Fixture.Type"))
                        .Excluding(d => d.SelectedMemberPath.EndsWith(".Mode.Fixture"))
                        .IgnoringCyclicReferences()
                )
            );
        }

        [Fact]
        public async Task GetDrawingAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .GetDrawingAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task GetPrintDrawingAsync() {
            PrintDrawing expected = new PrintDrawing {
                Drawing = testDrawings[0],
                UsedFixtures = new List<List<UsedFixtureResult>> {
                    new List<UsedFixtureResult> {
                        new UsedFixtureResult {
                            Fixture = testFixture,
                            Count = 1
                        }
                    }
                },
                RiggedFixtures = new List<RiggedFixture> {
                    testDrawings[0].Views[0].Structures[0].Fixtures[0]
                }
            };

            await _fixture.RunWithDatabaseAsync<PrintDrawing>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .GetPrintDrawingAsync(testDrawings[0].Id),
                (result, context) => result.Should().BeEquivalentTo(
                    expected,
                    options => options.IgnoringCyclicReferences()
                        .Excluding(d => d.SelectedMemberPath.EndsWith(".Owner"))
                        .Excluding(d => d.SelectedMemberPath.EndsWith(".Fixture.Type"))
                        .Excluding(d => d.SelectedMemberPath.EndsWith(".Fixture.Modes"))
                        .Excluding(d => d.SelectedMemberPath.EndsWith(".Mode.Fixture"))
                        .Excluding(d => d.SelectedMemberPath.Contains(".Structure.View."))
                        .Excluding(d => d.Drawing.UserDrawings)
                )
            );
        }

        [Fact]
        public async Task GetPrintDrawingAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .GetPrintDrawingAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task DeleteDrawingAsync() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .DeleteDrawingAsync(testDrawings[0].Id),
                context => context.Drawings.Where(d => d.Id == testDrawings[0].Id).ToList()
                    .Should().BeEmpty()
            );
        }

        [Fact]
        public async Task DeleteDrawingAsync_WithTemplate() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    context.Templates.Add(testTemplate);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .DeleteDrawingAsync(testDrawings[0].Id),
                context => context.Templates.Where(t => t.Id == testTemplate.Id).ToList()
                    .Should().BeEmpty()
            );
        }

        [Fact]
        public async Task DeleteDrawingAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .DeleteDrawingAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task DrawingExistsAsync_Exists() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .DrawingExistsAsync(testDrawings[0].Id),
                (result, context) => result.Should().Be(true)
            );
        }

        [Fact]
        public async Task DrawingExistsAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .DrawingExistsAsync("doesn't exist"),
                (result, context) => result.Should().Be(false)
            );
        }

        [Fact]
        public async Task GetSharedUsersAsync() {
            await _fixture.RunWithDatabaseAsync<List<UserDrawing>>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .GetSharedUsersAsync(testDrawings[1].Id),
                (result, context) => result.Should().HaveCount(1)
                    .And.BeEquivalentTo(
                        testDrawings[1].UserDrawings,
                        options => options.Excluding(ud => ud.Drawing)
                    )
            );
        }

        [Fact]
        public async Task GetSharedUsersAsync_NotShared() {
            await _fixture.RunWithDatabaseAsync<List<UserDrawing>>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .GetSharedUsersAsync(testDrawings[0].Id),
                (result, context) => result.Should().BeEmpty()
            );
        }

        [Fact]
        public async Task ShareWithUserAsync() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .ShareWithUserAsync(testUsers[1].Email, testDrawings[0].Id),
                context => context.UserDrawings.Where(ud => ud.Drawing.Id == testDrawings[0].Id).Where(ud => ud.User.Id == testUsers[1].Id).ToList()
                    .Should().HaveCount(1)
            );
        }

        [Fact]
        public async Task ShareWithUserAsync_EmailNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .ShareWithUserAsync("doesn't exist", testDrawings[0].Id),
                async (act, context) => await (act.Should().ThrowAsync<KeyNotFoundException>()).WithMessage("Email not found")
            );
        }

        [Fact]
        public async Task ShareWithUserAsync_DrawingNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .ShareWithUserAsync(testUsers[1].Email, "doesn't exist"),
                async (act, context) => await (act.Should().ThrowAsync<KeyNotFoundException>()).WithMessage("Drawing ID not found")
            );
        }

        [Fact]
        public async Task ShareWithUserAsync_WithOwner() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .ShareWithUserAsync(testUsers[0].Email, testDrawings[0].Id),
                async (act, context) => await (act.Should().ThrowAsync<InvalidOperationException>()).WithMessage("Cannot share with drawing owner")
            );
        }

        [Fact]
        public async Task ShareWithUserAsync_AlreadyShared() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .ShareWithUserAsync(testUsers[1].Email, testDrawings[1].Id),
                async (act, context) => await (act.Should().ThrowAsync<InvalidOperationException>()).WithMessage("Drawing already shared with user")
            );
        }

        [Fact]
        public async Task UnshareWithUserAsync() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.AddRange(testUsers);
                    context.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .UnshareWithUserAsync(testDrawings[1].UserDrawings[0].Id),
                context => context.UserDrawings.Where(ud => ud.Drawing.Id == testDrawings[1].Id).ToList()
                    .Should().NotContain(testDrawings[1].UserDrawings[0])
            );
        }

        [Fact]
        public async Task UnshareWithUserAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .UnshareWithUserAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task IsSharedWithAsync_Shared() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .IsSharedWithAsync(testDrawings[1].Id, testUsers[1].Id),
                (result, context) => result.Should().Be(true)
            );
        }

        [Fact]
        public async Task IsSharedWithAsync_NotShared() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .IsSharedWithAsync(testDrawings[0].Id, testUsers[1].Id),
                (result, context) => result.Should().Be(false)
            );
        }

        [Fact]
        public async Task IsOwnerAsync_IsOwner() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .IsOwnerAsync(testDrawings[0].Id, testUsers[0].Id),
                (result, context) => result.Should().Be(true)
            );
        }

        [Fact]
        public async Task IsOwnerAsync_NotOwner() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .IsOwnerAsync(testDrawings[0].Id, testUsers[1].Id),
                (result, context) => result.Should().Be(false)
            );
        }

        [Fact]
        public async Task GetOwnedDrawingsAsync() {
            await _fixture.RunWithDatabaseAsync<List<Drawing>>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .GetOwnedDrawingsAsync(testUsers[0].Id),
                (result, context) => result.Should().BeEquivalentTo(
                    testDrawings,
                    options => options.Excluding(d => d.Views)
                        .Excluding(d => d.UserDrawings)
                        .Excluding(d => d.Owner)
                )
            );
        }

        [Fact]
        public async Task GetOwnedDrawingsAsync_NoneOwned() {
            await _fixture.RunWithDatabaseAsync<List<Drawing>>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .GetOwnedDrawingsAsync(testUsers[1].Id),
                (result, context) => result.Should().BeEmpty()
            );
        }

        [Fact]
        public async Task GetOwnedDrawingsAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync<List<Drawing>>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .GetOwnedDrawingsAsync("doesn't exist"),
                (result, context) => result.Should().BeEmpty()
            );
        }

        [Fact]
        public async Task GetSharedDrawingsAsync() {
            await _fixture.RunWithDatabaseAsync<List<Drawing>>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .GetSharedDrawingsAsync(testUsers[1].Id),
                (result, context) => result.Should().AllBeEquivalentTo(
                    testDrawings[1],
                    options => options.Excluding(d => d.Views)
                        .Excluding(d => d.UserDrawings)
                        .Excluding(d => d.Owner)
                )
            );
        }

        [Fact]
        public async Task GetSharedDrawingsAsync_NoneShared() {
            await _fixture.RunWithDatabaseAsync<List<Drawing>>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .GetSharedDrawingsAsync(testUsers[0].Id),
                (result, context) => result.Should().BeEmpty()
            );
        }

        [Fact]
        public async Task GetSharedDrawingsAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync<List<Drawing>>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .GetSharedDrawingsAsync("doesn't exist"),
                (result, context) => result.Should().BeEmpty()
            );
        }

        [Fact]
        public async Task UpdateLastModifiedAsync() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .UpdateLastModifiedAsync(testDrawings[0]),
                context => context.Drawings.First(d => d.Id == testDrawings[0].Id).LastModified
                    .Should().BeCloseTo(DateTime.Now)
            );
        }

        [Fact]
        public async Task UpdateLastModifiedAsync_NotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .UpdateLastModifiedAsync(new Drawing { Id = "doesn't exist" }),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }
    }
}