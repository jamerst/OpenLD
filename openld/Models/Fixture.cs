using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace openld.Models {
    public class Fixture {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; } = "Unknown";
        public DateTime ReleaseDate { get; set; }
        public FixtureType Type { get; set; }
        public int Power { get; set; }
        public float Weight { get; set; }
        public StoredImage Image { get; set; }
        public Symbol Symbol { get; set; }
        public List<FixtureMode> Modes { get; set; }
    }
}