using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace openld.Models {
    public class Fixture {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public DateTime ReleaseData { get; set; }
        public FixtureType Type { get; set; }
        public int Power { get; set; }
        public float Weight { get; set; }
        [Column(TypeName = "xml")]
        public string Symbol { get; set; }
    }
}