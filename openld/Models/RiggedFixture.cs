using System.ComponentModel.DataAnnotations.Schema;

namespace openld.Models {
    public class RiggedFixture {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public Fixture Fixture { get; set; }
        public Structure Structure { get; set; }
        [Column(TypeName = "jsonb")]
        public Point Position { get; set; }
        public short Angle { get; set; }
        public short Channel { get; set; }
        public short Address { get; set; }
        public short Universe { get; set; }
        public FixtureMode Mode { get; set; }
        public string Notes { get; set; }
        public string Colour { get; set; }
        public RiggedFixture Clone() {
            return new RiggedFixture {
                Id = null,
                Name = this.Name,
                Fixture = this.Fixture,
                Structure = null,
                Position = this.Position,
                Angle = this.Angle,
                Channel = this.Channel,
                Address = this.Address,
                Universe = this.Universe,
                Mode = this.Mode,
                Notes = this.Notes,
                Colour = this.Colour
            };
        }
    }
}