using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using FluentAssertions;
using Xunit;

using openld.Models;
using openld.Services;

namespace openld.Tests
{
    public class FixtureTypeServiceTest {
        private readonly TestFixture _fixture = new TestFixture();

        [Fact]
        public async Task GetAllFixtureTypes() {
            FixtureType[] newTypes = {
                new FixtureType {Id = "testing1", Name = "testType1"},
                new FixtureType {Id = "testing2", Name = "testType2"},
                new FixtureType {Id = "testing3", Name = "testType3"},
            };

            await _fixture.RunWithDatabaseAsync<List<FixtureType>>(
                async context => {
                    context.FixtureTypes.AddRange(newTypes);
                    await context.SaveChangesAsync();
                },
                context => new FixtureTypeService(context).GetAllTypesAsync(),
                (result, context) => result.Should()
                    .BeOfType<List<FixtureType>>()
                    .And.HaveCount(newTypes.Length)
            );
        }

        [Fact]
        public async Task CreateTypes() {
            string[] newTypes = {
                "testType1",
                "testType2",
                "testType3"
            };

            await _fixture.RunWithDatabaseAsync(
                null,
                context => new FixtureTypeService(context).CreateTypesAsync(newTypes),
                context => context.FixtureTypes.ToList().Should()
                    .HaveCount(newTypes.Length)
                    .And.Contain(t => newTypes.Contains(t.Name))
            );
        }
    }
}
