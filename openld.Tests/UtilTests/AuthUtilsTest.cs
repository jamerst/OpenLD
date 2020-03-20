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
using openld.Utils;

namespace openld.Tests {
    public class AuthUtilsTest : OpenLDUnitTest {
        private static AuthUtils initUtils(OpenLDContext context) {
            ViewService viewService = new ViewService(context);
            TemplateService templateService = new TemplateService(context);
            LabelService labelService = new LabelService(context, viewService, _mapper);
            DrawingService drawingService = new DrawingService(context, templateService, viewService, _mapper);
            StructureService structureService = new StructureService(context, drawingService, viewService, _mapper);
            RiggedFixtureService rFixtureService = new RiggedFixtureService(context, structureService, _mapper);
            return new AuthUtils(
                drawingService,
                labelService,
                rFixtureService,
                structureService,
                viewService
            );
        }

        [Fact]
        public async Task hasAccess_Drawing_Owner() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[0].Id, testUsers[0].Id),
                (result, context) => result.Should().BeTrue()
            );
        }

        [Fact]
        public async Task hasAccess_Drawing_Shared() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[1].Id, testUsers[1].Id),
                (result, context) => result.Should().BeTrue()
            );
        }

        [Fact]
        public async Task hasAccess_Drawing_NoAccess() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[0].Id, testUsers[1].Id),
                (result, context) => result.Should().BeFalse()
            );
        }

        [Fact]
        public async Task hasAccess_View_Owner() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[0].Views[0], testUsers[0].Id),
                (result, context) => result.Should().BeTrue()
            );
        }

        [Fact]
        public async Task hasAccess_View_Shared() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[1].Views[0], testUsers[1].Id),
                (result, context) => result.Should().BeTrue()
            );
        }

        [Fact]
        public async Task hasAccess_View_NoAccess() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[0].Views[0], testUsers[1].Id),
                (result, context) => result.Should().BeFalse()
            );
        }

        [Fact]
        public async Task hasAccess_Label_Owner() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[0].Views[0].Labels[0], testUsers[0].Id),
                (result, context) => result.Should().BeTrue()
            );
        }

        [Fact]
        public async Task hasAccess_Label_Shared() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[1].Views[0].Labels[0], testUsers[1].Id),
                (result, context) => result.Should().BeTrue()
            );
        }

        [Fact]
        public async Task hasAccess_Label_NoAccess() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[0].Views[0].Labels[0], testUsers[1].Id),
                (result, context) => result.Should().BeFalse()
            );
        }

        [Fact]
        public async Task hasAccess_Structure_Owner() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[0].Views[0].Structures[0], testUsers[0].Id),
                (result, context) => result.Should().BeTrue()
            );
        }

        [Fact]
        public async Task hasAccess_Structure_Shared() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[1].Views[0].Structures[0], testUsers[1].Id),
                (result, context) => result.Should().BeTrue()
            );
        }

        [Fact]
        public async Task hasAccess_Structure_NoAccess() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[0].Views[0].Structures[0], testUsers[1].Id),
                (result, context) => result.Should().BeFalse()
            );
        }
        [Fact]
        public async Task hasAccess_rFixture_Owner() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[0].Views[0].Structures[0].Fixtures[0], testUsers[0].Id),
                (result, context) => result.Should().BeTrue()
            );
        }

        [Fact]
        public async Task hasAccess_rFixture_Shared() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[1].Views[0].Structures[0].Fixtures[0], testUsers[1].Id),
                (result, context) => result.Should().BeTrue()
            );
        }

        [Fact]
        public async Task hasAccess_rFixture_NoAccess() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initUtils(context).hasAccess(testDrawings[0].Views[0].Structures[0].Fixtures[0], testUsers[1].Id),
                (result, context) => result.Should().BeFalse()
            );
        }
    }
}