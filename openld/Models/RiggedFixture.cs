using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace openld.Models {
    public class RiggedFixture {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public Fixture Fixture { get; set; }
        public Structure Structure { get; set; }
        public Point Position { get; set; }
        public short Address { get; set; }
        public short Universe { get; set; }
        public FixtureMode Mode { get; set; }
        public string Notes { get; set; }
    }
}