using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using IdentityServer4.EntityFramework.Options;
using Microsoft.Extensions.Options;
using Npgsql;

using openld.Models;

namespace openld.Data {
    public class OpenLDContext : ApiAuthorizationDbContext<User> {
        public OpenLDContext(DbContextOptions<OpenLDContext> options, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions) {}

        protected override void OnConfiguring(DbContextOptionsBuilder builder) {
            builder.UseNpgsql("Host=db; Database=openld_db; Username=openld; Password=openld");
            // use JSON.NET for JSON type mapping, not System.Text.Json
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet();
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
        }

        public override DbSet<User> Users { get; set; }
        public DbSet<UserDrawings> UserDrawings { get; set; }
        public DbSet<Drawing> Drawings { get; set; }
        public DbSet<View> Views { get; set; }
        public DbSet<Structure> Structures { get; set; }
        public DbSet<StructureType> StructureTypes { get; set; }
        public DbSet<RiggedFixture> RiggedFixtures { get; set; }
        public DbSet<Models.Fixture> Fixtures { get; set; }
        public DbSet<FixtureMode> FixtureModes { get; set; }
        public DbSet<FixtureType> FixtureTypes { get; set; }
        public DbSet<StoredImage> StoredImages { get; set; }
    }
}