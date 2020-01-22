using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

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
        [Column(TypeName = "xml")]
        [JsonIgnore]
        public string Symbol { get; set; } = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 27 27\"><defs/><g fill=\"none\" stroke=\"#000\" stroke-width=\"3\"><circle cx=\"150.1\" cy=\"952.4\" r=\"50\" transform=\"matrix(.26436 0 0 .26458 -26 -238)\"/><path d=\"M115 987l70-70M115 917l70 70\" transform=\"matrix(.26436 0 0 .26458 -26 -238)\"/></g></svg>";
        public StoredImage Image { get; set; }
        public List<FixtureMode> Modes { get; set; }
    }
}