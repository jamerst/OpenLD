using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.Options;
using Microsoft.Extensions.Options;

using openld.Data;

namespace openld.Tests {
    public class TestFixture {
        public async Task RunWithDatabaseAsync<T>(
            Func<OpenLDContext, Task> arrange,
            Func<OpenLDContext, Task<T>> act,
            Action<T, OpenLDContext> assert
        ) {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            try {
                var options = new DbContextOptionsBuilder<OpenLDContext>()
                    .UseSqlite(connection);

                using (var context = new OpenLDTestContext(options.Options)) {
                    await context.Database.EnsureCreatedAsync();
                    if (arrange != null) {
                        await arrange.Invoke(context);
                    }
                }

                using (var context = new OpenLDTestContext(options.Options)) {
                    var result = await act(context);
                    assert(result, context);
                }
            } finally {
                await connection.CloseAsync();
            }
        }

        public async Task RunWithDatabaseAsync(
            Func<OpenLDContext, Task> arrange,
            Func<OpenLDContext, Task> act,
            Action<OpenLDContext> assert
        ) {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            try {
                var options = new DbContextOptionsBuilder<OpenLDContext>()
                    .UseSqlite(connection);

                using (var context = new OpenLDTestContext(options.Options)) {
                    await context.Database.EnsureCreatedAsync();
                    if (arrange != null) {
                        await arrange.Invoke(context);
                    }
                }

                using (var context = new OpenLDTestContext(options.Options)) {
                    await act(context);
                    assert(context);
                }
            } finally {
                await connection.CloseAsync();
            }
        }

        public async Task RunWithDatabaseAsync(
            Func<OpenLDContext, Task> arrange,
            Func<OpenLDContext, Task> act,
            Func<Func<Task>, OpenLDContext, Task> assert
        ) {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            try {
                var options = new DbContextOptionsBuilder<OpenLDContext>()
                    .UseSqlite(connection);

                using (var context = new OpenLDTestContext(options.Options)) {
                    await context.Database.EnsureCreatedAsync();
                    if (arrange != null) {
                        await arrange.Invoke(context);
                    }
                }

                using (var context = new OpenLDTestContext(options.Options)) {
                    Func<Task> executeAct = async () => await act(context);
                    await assert(executeAct, context);
                }
            } finally {
                await connection.CloseAsync();
            }
        }
    }
}