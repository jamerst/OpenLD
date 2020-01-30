using System.ComponentModel.DataAnnotations.Schema;

namespace openld.Models {
    public class FixtureMode {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public Fixture Fixture { get; set; }
        public int Channels { get; set; }
    }
}