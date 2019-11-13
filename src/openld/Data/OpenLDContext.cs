using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using openld.Models;

namespace openld.Data {
    public class OpenLDContext : DbContext {
        public OpenLDContext(DbContextOptions<OpenLDContext> options) : base(options) {}

        protected override void OnConfiguring(DbContextOptionsBuilder builder) {
            builder.UseNpgsql("Host=db; Database=openld_db; Username=openld; Password=openld",
                o => o.UseNetTopologySuite());
        }

        protected override void OnModelCreating(ModelBuilder builder) {
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
    }
}