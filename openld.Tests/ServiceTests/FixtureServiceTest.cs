using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

using openld.Models;
using openld.Services;

namespace openld.Tests
{
    public class FixtureServiceTest {
        private readonly TestFixture _fixture = new TestFixture();

        private static FixtureType[] testTypes = {
            new FixtureType {Id = "testType1", Name = "testType1"},
            new FixtureType {Id = "testType2", Name = "testType2"},
            new FixtureType {Id = "testType3", Name = "testType3"},
        };

        private static Fixture[] testFixtures = {
            new Fixture {
                Id = "testFixture1",
                Name = "testFixture1",
                Manufacturer = "testManf1",
                Type = testTypes[0],
                Power = 100,
                Weight = 3.5F
            },
            new Fixture {
                Id = "testFixture2",
                Name = "testFixture2",
                Manufacturer = "testManf1",
                Type = testTypes[0],
                Power = 100,
                Weight = 3.5F
            },
            new Fixture {
                Id = "testFixture3",
                Name = "testFixture3",
                Manufacturer = "testManf2",
                Type = testTypes[2],
                Power = 100,
                Weight = 3.5F
            }
        };

        private static StoredImage[] testImages = {
            new StoredImage {
                Id = "testFixture4Image"
            },
            new StoredImage {
                Id = "testFixture4SymbolBitmap"
            }
        };

        private static Symbol[] testSymbols = {
            new Symbol {
                Id = "testFixture4Symbol",
                Bitmap = testImages[1]
            }
        };

        [Fact]
        public async Task SearchFixturesNoParams() {
            await _fixture.RunWithDatabaseAsync<List<Fixture>>(
                async context => {
                    context.FixtureTypes.AddRange(testTypes);
                    context.Fixtures.AddRange(testFixtures);
                    await context.SaveChangesAsync();
                },
                context => new FixtureService(context).SearchAllFixturesAsync(
                    new Utils.SearchParams {name = "", manufacturer = "", type =""}
                ),
                (result, context) => result.Should()
                    .BeOfType<List<Fixture>>()
                    .And.HaveCount(testFixtures.Length)
            );
        }

        [Fact]
        public async Task SearchFixtureName() {
            await _fixture.RunWithDatabaseAsync<List<Fixture>>(
                async context => {
                    context.FixtureTypes.AddRange(testTypes);
                    context.Fixtures.AddRange(testFixtures);
                    await context.SaveChangesAsync();
                },
                context => new FixtureService(context).SearchAllFixturesAsync(
                    new Utils.SearchParams {name = "testfixture1", manufacturer = "", type = ""}
                ),
                (result, context) => result.Should()
                    .BeOfType<List<Fixture>>()
                    .And.HaveCount(1)
            );
        }

        [Fact]
        public async Task SearchFixtureNameManf() {
            await _fixture.RunWithDatabaseAsync<List<Fixture>>(
                async context => {
                    context.FixtureTypes.AddRange(testTypes);
                    context.Fixtures.AddRange(testFixtures);
                    await context.SaveChangesAsync();
                },
                context => new FixtureService(context).SearchAllFixturesAsync(
                    new Utils.SearchParams {name = "Testfixture1", manufacturer = "testmanf1", type = ""}
                ),
                (result, context) => result.Should()
                    .BeOfType<List<Fixture>>()
                    .And.HaveCount(1)
            );
        }

        [Fact]
        public async Task SearchFixtureManf() {
            await _fixture.RunWithDatabaseAsync<List<Fixture>>(
                async context => {
                    context.FixtureTypes.AddRange(testTypes);
                    context.Fixtures.AddRange(testFixtures);
                    await context.SaveChangesAsync();
                },
                context => new FixtureService(context).SearchAllFixturesAsync(
                    new Utils.SearchParams {name = "", manufacturer = "Testmanf1", type = ""}
                ),
                (result, context) => result.Should()
                    .BeOfType<List<Fixture>>()
                    .And.HaveCount(2)
            );
        }

        [Fact]
        public async Task SearchFixtureTypeEmpty() {
            await _fixture.RunWithDatabaseAsync<List<Fixture>>(
                async context => {
                    context.FixtureTypes.AddRange(testTypes);
                    context.Fixtures.AddRange(testFixtures);
                    await context.SaveChangesAsync();
                },
                context => new FixtureService(context).SearchAllFixturesAsync(
                    new Utils.SearchParams {name = "", manufacturer = "", type = "testType2"}
                ),
                (result, context) => result.Should()
                    .BeOfType<List<Fixture>>()
                    .And.BeEmpty()
            );
        }

        [Fact]
        public async Task SearchFixtureType() {
            await _fixture.RunWithDatabaseAsync<List<Fixture>>(
                async context => {
                    context.FixtureTypes.AddRange(testTypes);
                    context.Fixtures.AddRange(testFixtures);
                    await context.SaveChangesAsync();
                },
                context => new FixtureService(context).SearchAllFixturesAsync(
                    new Utils.SearchParams {name = "", manufacturer = "", type = "testType1"}
                ),
                (result, context) => result.Should()
                    .BeOfType<List<Fixture>>()
                    .And.HaveCount(2)
            );
        }

        [Fact]
        public async Task SearchFixtureNameType() {
            await _fixture.RunWithDatabaseAsync<List<Fixture>>(
                async context => {
                    context.FixtureTypes.AddRange(testTypes);
                    context.Fixtures.AddRange(testFixtures);
                    await context.SaveChangesAsync();
                },
                context => new FixtureService(context).SearchAllFixturesAsync(
                    new Utils.SearchParams {name = "testfixture2", manufacturer = "", type = "testType1"}
                ),
                (result, context) => result.Should()
                    .BeOfType<List<Fixture>>()
                    .And.HaveCount(1)
            );
        }

        [Fact]
        public async Task SearchFixtureNoResults() {
            await _fixture.RunWithDatabaseAsync<List<Fixture>>(
                async context => {
                    context.FixtureTypes.AddRange(testTypes);
                    context.Fixtures.AddRange(testFixtures);
                    await context.SaveChangesAsync();
                },
                context => new FixtureService(context).SearchAllFixturesAsync(
                    new Utils.SearchParams {name = "doesn't exist", manufacturer = "doesn't exist", type = "doesn't exist"}
                ),
                (result, context) => result.Should()
                    .BeOfType<List<Fixture>>()
                    .And.BeEmpty()
            );
        }

        [Fact]
        public async Task CreateFixtureNoMode() {
            Fixture newFixture = new Fixture {
                Id = "testFixture4",
                Name = "testFixture4",
                Manufacturer = "testManf1",
                Type = testTypes[0],
                Power = 100,
                Weight = 5F,
                Image = testImages[0],
                Symbol = testSymbols[0],
                Modes = null
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.FixtureTypes.AddRange(testTypes);
                    context.Fixtures.AddRange(testFixtures);
                    context.StoredImages.AddRange(testImages);
                    context.Symbols.AddRange(testSymbols);
                    await context.SaveChangesAsync();
                },
                context => new FixtureService(context).CreateFixtureAsync(newFixture),
                context => context.Fixtures.Include(f => f.Image).Include(f => f.Symbol).Include(f => f.Modes)
                    .Where(f => f.Id == newFixture.Id).ToList().Should()
                    .HaveCount(1)
                    .And.Equal(new List<Fixture> {newFixture})
            );
        }

        [Fact]
        public async Task CreateFixtureWithModes() {
            List<FixtureMode> newModes = new List<FixtureMode> {
                new FixtureMode {
                    Id = "testFixture4Mode1",
                    Name = "testFixture4Mode1",
                    Channels = 1
                },
                new FixtureMode {
                    Id = "testFixture4Mode2",
                    Name = "testFixture4Mode2",
                    Channels = 2
                },
                new FixtureMode {
                    Id = "testFixture4Mode3",
                    Name = "testFixture4Mode3",
                    Channels = 3
                }
            };

            Fixture newFixture = new Fixture {
                Id = "testFixture4",
                Name = "testFixture4",
                Manufacturer = "testManf1",
                Type = testTypes[0],
                Power = 100,
                Weight = 5F,
                Image = testImages[0],
                Symbol = testSymbols[0],
                Modes = newModes
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.FixtureTypes.AddRange(testTypes);
                    context.Fixtures.AddRange(testFixtures);
                    context.StoredImages.AddRange(testImages);
                    context.Symbols.AddRange(testSymbols);
                    await context.SaveChangesAsync();
                },
                context => new FixtureService(context).CreateFixtureAsync(newFixture),
                context => context.Fixtures.Include(f => f.Image).Include(f => f.Symbol).Include(f => f.Modes)
                    .Where(f => f.Id == newFixture.Id).ToList().Should()
                    .HaveCount(1)
                    .And.Equal(new List<Fixture> {newFixture})
            );
        }

        [Fact]
        public async Task CreateFixtureNoType() {
            List<FixtureMode> newModes = new List<FixtureMode> {
                new FixtureMode {
                    Id = "testFixture4Mode1",
                    Name = "testFixture4Mode1",
                    Channels = 1
                },
                new FixtureMode {
                    Id = "testFixture4Mode2",
                    Name = "testFixture4Mode2",
                    Channels = 2
                },
                new FixtureMode {
                    Id = "testFixture4Mode3",
                    Name = "testFixture4Mode3",
                    Channels = 3
                }
            };

            Fixture newFixture = new Fixture {
                Id = "testFixture4",
                Name = "testFixture4",
                Manufacturer = "testManf1",
                Type = new FixtureType {Id = "doesn't exist"},
                Power = 100,
                Weight = 5F,
                Image = testImages[0],
                Symbol = testSymbols[0],
                Modes = newModes
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.FixtureTypes.AddRange(testTypes);
                    context.Fixtures.AddRange(testFixtures);
                    context.StoredImages.AddRange(testImages);
                    context.Symbols.AddRange(testSymbols);
                    await context.SaveChangesAsync();
                },
                context => new FixtureService(context).CreateFixtureAsync(newFixture),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

         [Fact]
        public async Task CreateFixtureNoImage() {
            List<FixtureMode> newModes = new List<FixtureMode> {
                new FixtureMode {
                    Id = "testFixture4Mode1",
                    Name = "testFixture4Mode1",
                    Channels = 1
                },
                new FixtureMode {
                    Id = "testFixture4Mode2",
                    Name = "testFixture4Mode2",
                    Channels = 2
                },
                new FixtureMode {
                    Id = "testFixture4Mode3",
                    Name = "testFixture4Mode3",
                    Channels = 3
                }
            };

            Fixture newFixture = new Fixture {
                Id = "testFixture4",
                Name = "testFixture4",
                Manufacturer = "testManf1",
                Type = testTypes[0],
                Power = 100,
                Weight = 5F,
                Image = new StoredImage {Id = "doesn't exist"},
                Symbol = testSymbols[0],
                Modes = newModes
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.FixtureTypes.AddRange(testTypes);
                    context.Fixtures.AddRange(testFixtures);
                    context.StoredImages.AddRange(testImages);
                    context.Symbols.AddRange(testSymbols);
                    await context.SaveChangesAsync();
                },
                context => new FixtureService(context).CreateFixtureAsync(newFixture),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }

         [Fact]
        public async Task CreateFixtureNoSymbol() {
            List<FixtureMode> newModes = new List<FixtureMode> {
                new FixtureMode {
                    Id = "testFixture4Mode1",
                    Name = "testFixture4Mode1",
                    Channels = 1
                },
                new FixtureMode {
                    Id = "testFixture4Mode2",
                    Name = "testFixture4Mode2",
                    Channels = 2
                },
                new FixtureMode {
                    Id = "testFixture4Mode3",
                    Name = "testFixture4Mode3",
                    Channels = 3
                }
            };

            Fixture newFixture = new Fixture {
                Id = "testFixture4",
                Name = "testFixture4",
                Manufacturer = "testManf1",
                Type = testTypes[0],
                Power = 100,
                Weight = 5F,
                Image = testImages[0],
                Symbol = new Symbol {Id = "doesn't exist"},
                Modes = newModes
            };

            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.FixtureTypes.AddRange(testTypes);
                    context.Fixtures.AddRange(testFixtures);
                    context.StoredImages.AddRange(testImages);
                    context.Symbols.AddRange(testSymbols);
                    await context.SaveChangesAsync();
                },
                context => new FixtureService(context).CreateFixtureAsync(newFixture),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }
    }
}
