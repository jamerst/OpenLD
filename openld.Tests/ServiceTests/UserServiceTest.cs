using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using openld.Data;
using openld.Models;
using openld.Services;

namespace openld.Tests {
    public class UserServiceTest : OpenLDUnitTest {
        private static UserService initService(OpenLDContext context) {
            return new UserService(context);
        }
        [Fact]
        public async Task GetUserDetailsExisting() {
            await _fixture.RunWithDatabaseAsync<User>(
                async context => {
                    context.Users.AddRange(testUsers);
                    await context.SaveChangesAsync();
                },
                context => initService(context).GetUserDetailsAsync("user1"),
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
                context => initService(context).GetUserDetailsAsync("doesn't exist"),
                async (act, context) => await act.Should().ThrowAsync<KeyNotFoundException>()
            );
        }
    }
}