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
    public class TemplateServiceTest : OpenLDUnitTest {
        private static TemplateService initService(OpenLDContext context) {
            return new TemplateService(context);
        }

        [Fact]
        public async Task GetTemplatesAsync() {
            await _fixture.RunWithDatabaseAsync<List<Template>>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    context.Templates.Add(testTemplate);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetTemplatesAsync(),
                (result, context) => result.Should().HaveCount(1)
                    .And.AllBeEquivalentTo(
                        testTemplate,
                        options => options.Excluding(t => t.Drawing.Owner)
                            .Excluding(t => t.Drawing.Views)
                    )
            );
        }

        [Fact]
        public async Task CreateTemplateAsync() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    await context.SaveChangesAsync();
                },
                context => initService(context).CreateTemplateAsync(testDrawings[1].Id),
                context => context.Templates.Where(t => t.Drawing.Id == testDrawings[1].Id).ToList()
                    .Should().HaveCount(1)
            );
        }

        [Fact]
        public async Task CreateTemplateAsync_DrawingNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    context.Templates.Add(testTemplate);
                    await context.SaveChangesAsync();
                },
                context => initService(context).CreateTemplateAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

        [Fact]
        public async Task CreateTemplateAsync_TemplateAlreadyExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    context.Templates.Add(testTemplate);
                    await context.SaveChangesAsync();
                },
                context => initService(context).CreateTemplateAsync(testDrawings[0].Id),
                async (act, context) => await act.Should().ThrowAsync<InvalidOperationException>()
            );
        }

        [Fact]
        public async Task IsTemplateAsync_IsTemplate() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    context.Templates.Add(testTemplate);
                    await context.SaveChangesAsync();
                },
                context => initService(context).IsTemplateAsync(testDrawings[0].Id),
                (result, context) => result.Should().Be(true)
            );
        }

        [Fact]
        public async Task IsTemplateAsync_IsNotTemplate() {
            await _fixture.RunWithDatabaseAsync<bool>(
                async context => {
                    context.Drawings.AddRange(testDrawings);
                    context.Templates.Add(testTemplate);
                    await context.SaveChangesAsync();
                },
                context => initService(context).IsTemplateAsync(testDrawings[1].Id),
                (result, context) => result.Should().Be(false)
            );
        }
    }
}