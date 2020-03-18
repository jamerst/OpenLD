using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.Options;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using openld.Data;
using openld.Models;

namespace openld.Tests {
    public class OpenLDTestContext : OpenLDContext {
        public OpenLDTestContext(DbContextOptions<OpenLDContext> options) : base(options, Options.Create<OperationalStoreOptions>(new OperationalStoreOptions())) {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            // manually convert to and from JSON string as SQLite doesn't support JSON column type
            builder.Entity<Label>()
                .Property(l => l.Position)
                .HasColumnType("TEXT")
                .HasConversion(
                    p => JsonConvert.SerializeObject(p),
                    p => JsonConvert.DeserializeObject<Point>(p)
                );

            builder.Entity<RiggedFixture>()
                .Property(f => f.Position)
                .HasColumnType("TEXT")
                .HasConversion(
                    p => JsonConvert.SerializeObject(p),
                    p => JsonConvert.DeserializeObject<Point>(p)
                );

            builder.Entity<Structure>()
                .Property(s => s.Geometry)
                .HasColumnType("TEXT")
                .HasConversion(
                    p => JsonConvert.SerializeObject(p),
                    p => JsonConvert.DeserializeObject<Geometry>(p)
                );
        }
    }
}