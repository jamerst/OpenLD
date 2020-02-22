using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using IdentityServer4.EntityFramework.Options;
using Microsoft.Extensions.Options;

using openld.Models;

namespace openld.Data {
    public class OpenLDContext : ApiAuthorizationDbContext<User> {
        public OpenLDContext(DbContextOptions<OpenLDContext> options, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions) {}

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<Drawing>()
                .HasMany(d => d.Views)
                .WithOne(v => v.Drawing)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Drawing>()
                .HasMany(d => d.UserDrawings)
                .WithOne(ud => ud.Drawing)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<View>()
                .HasMany(v => v.Structures)
                .WithOne(s => s.View)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<View>()
                .HasMany(v => v.Labels)
                .WithOne(l => l.View)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Structure>()
                .HasMany(s => s.Fixtures)
                .WithOne(f => f.Structure)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Fixture>()
                .HasMany(f => f.Modes)
                .WithOne(m => m.Fixture)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        public override DbSet<User> Users { get; set; }
        public DbSet<UserDrawing> UserDrawings { get; set; }
        public DbSet<Drawing> Drawings { get; set; }
        public DbSet<View> Views { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Structure> Structures { get; set; }
        public DbSet<StructureType> StructureTypes { get; set; }
        public DbSet<RiggedFixture> RiggedFixtures { get; set; }
        public DbSet<Models.Fixture> Fixtures { get; set; }
        public DbSet<FixtureMode> FixtureModes { get; set; }
        public DbSet<FixtureType> FixtureTypes { get; set; }
        public DbSet<StoredImage> StoredImages { get; set; }
        public DbSet<Symbol> Symbols { get; set; }
        public DbSet<Template> Templates { get; set; }
    }
}