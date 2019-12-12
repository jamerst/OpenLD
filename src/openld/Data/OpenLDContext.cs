using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using IdentityServer4.EntityFramework.Options;
using Microsoft.Extensions.Options;
using openld.Models;

namespace openld.Data {
    public class OpenLDContext : ApiAuthorizationDbContext<User> {
        public OpenLDContext(DbContextOptions<OpenLDContext> options, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions) {}

        protected override void OnConfiguring(DbContextOptionsBuilder builder) {
            builder.UseNpgsql("Host=db; Database=openld_db; Username=openld; Password=openld",
                o => o.UseNetTopologySuite());
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            builder.HasPostgresExtension("postgis");
        }

        public DbSet<User> User { get; set; }
        public DbSet<UserDrawings> UserDrawings { get; set; }
        public DbSet<Drawing> Drawing { get; set; }
        public DbSet<View> View { get; set; }
        public DbSet<Structure> Structure { get; set; }
        public DbSet<StructureType> StructureType { get; set; }
        public DbSet<RiggedFixture> RiggedFixture { get; set; }
        public DbSet<Fixture> Fixture { get; set; }
        public DbSet<FixtureMode> FixtureMode { get; set; }
        public DbSet<FixtureType> FixtureType { get; set; }
        public DbSet<StoredImage> StoredImages { get; set; }
    }
}