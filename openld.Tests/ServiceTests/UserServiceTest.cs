using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using openld.Models;
using openld.Services;

namespace openld.Tests {
    public class UserServiceTest {
        private readonly TestFixture _fixture = new TestFixture();

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