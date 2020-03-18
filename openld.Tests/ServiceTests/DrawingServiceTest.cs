using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

using openld.Models;
using openld.Services;

namespace openld.Tests {
    public class DrawingServiceTest {
        private readonly TestFixture _fixture = new TestFixture();

        private readonly IMapper _mapper = new MapperConfiguration(cfg => {
            cfg.AddProfile<LabelProfile>();
            cfg.AddProfile<RiggedFixtureProfile>();
            cfg.AddProfile<StructureProfile>();
        }).CreateMapper();

        private static User[] testUsers = {
            new User {
                Id = "user1",
                UserName = "user1",
                Email = "userEmail1"
            },
            new User {
                Id = "user2",
                UserName = "user2",
                Email = "userEmail2"
            }
        };

        private static Fixture testFixture = new Fixture {
            Id = "testFixture1",
            Name = "testFixture1",
            Manufacturer = "testManf1",
            Type = new FixtureType { Id = "testType1", Name = "testType1" },
            Power = 100,
            Weight = 3.5F,
            Modes = new List<FixtureMode> {
                new FixtureMode {
                    Id = "testFixture1_Mode1",
                    Name = "testFixture1_Mode1",
                    Channels = 10
                },
                new FixtureMode {
                    Id = "testFixture1_Mode2",
                    Name = "testFixture1_Mode2",
                    Channels = 20
                },
            }
        };

        private static Drawing testDrawing = new Drawing {
            Id = "testDrawing1",
            Title = "testDrawing1",
            Owner = testUsers[0],
            Views = new List<View> {
                new View {
                    Id = "testDrawing1_View1",
                    Name = "testDrawing1_View1",
                    Structures = new List<Structure> {
                        new Structure {
                            Id = "testDrawing1_View1_Struct1",
                            Name = "testDrawing1_View1_Struct1",
                            Geometry = new Geometry {
                                Points = new List<Point> {
                                    new Point {x = 1, y = 1},
                                    new Point {x = 2, y = 2}
                                }
                            },
                            Fixtures = new List<RiggedFixture> {
                                new RiggedFixture {
                                    Id = "testDrawing1_View1_Struct1_RFixture1",
                                    Name = "testDrawing1_View1_Struct1_RFixture1",
                                    Position = new Point {x = 1.5, y = 1.5},
                                    Fixture = testFixture,
                                    Mode = testFixture.Modes[0],
                                    Angle = 0,
                                    Channel = 1,
                                    Address = 20,
                                    Universe = 1,
                                    Colour = "L202",
                                    Notes = ""
                                }
                            }
                        }
                    },
                    Labels = new List<Label> {
                        new Label {
                            Id = "testDrawing1_View1_Label1",
                            Position = new Point {x = 1.0, y = 1},
                            Text = "testDrawing1_View1_Label1"
                        },
                        new Label {
                            Id = "testDrawing1_View1_Label2",
                            Position = new Point {x = 1.0, y = 1},
                            Text = "testDrawing1_View1_Label2"
                        }
                    }
                }
            }
        };

        private static Template testTemplate = new Template {
            Id = "testDrawing1_Template",
            Drawing = testDrawing
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

            Template template = new Template {
                Id = "testDrawing1_Template"
            };

            await _fixture.RunWithDatabaseAsync<string>(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.Add(testDrawing);
                    context.Templates.Add(testTemplate);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .CreateDrawingFromTemplateAsync("user1", newDrawing, template),

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
                        testDrawing,
                        options => options
                            .Excluding(d => d.SelectedMemberPath.EndsWith("Id") && !d.SelectedMemberPath.Contains(".Fixture."))
                            .Excluding(d => d.Title)
                            .Excluding(d => d.LastModified)
                            .IgnoringCyclicReferences()
                    );
                }
            );
        }

        [Fact]
        public async Task CreateDrawingFromTemplateAsync_InvalidUser() {
            Drawing newDrawing = new Drawing {
                Id = "newDrawing1",
                Title = "newDrawing1"
            };

            Template template = new Template {
                Id = "testDrawing1_Template"
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    context.Drawings.Add(testDrawing);
                    context.Templates.Add(testTemplate);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .CreateDrawingFromTemplateAsync("doesn't exist", newDrawing, template),

                async (act, context) => await act.Should().ThrowAsync<UnauthorizedAccessException>()
            );
        }

        [Fact]
        public async Task CreateDrawingFromTemplateAsync_InvalidTemplate() {
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
                    context.Drawings.Add(testDrawing);
                    context.Templates.Add(testTemplate);
                    await context.SaveChangesAsync();
                },
                context => new DrawingService(context, new TemplateService(context), new ViewService(context), _mapper)
                    .CreateDrawingFromTemplateAsync("user1", newDrawing, template),

                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }
    }
}