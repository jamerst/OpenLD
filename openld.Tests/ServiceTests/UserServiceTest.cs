using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using openld.Models;
using openld.Services;

namespace openld.Tests {
    public class UserServiceTest : OpenLDUnitTest {
        [Fact]
        public async Task GetUserDetailsExisting() {
            await _fixture.RunWithDatabaseAsync<User>(
                async context => {
                    context.Users.AddRange(testUsers);
                    await context.SaveChangesAsync();
                },
                context => new UserService(context).GetUserDetailsAsync("user1"),
                (result, context) => result.Should().Equals(testUsers[0])
            );
        }

        [Fact]
        public async Task GetUserDetailsNotExists() {
            await _fixture.RunWithDatabaseAsync(
                async context => {
                    context.Users.AddRange(testUsers);
                    await context.SaveChangesAsync();
                },
                context => new UserService(context).GetUserDetailsAsync("user3"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }
    }
}