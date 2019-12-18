using System;

namespace openld.Data.Entities {
    public class Fixture {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string TypeId { get; set; }
        public int Power { get; set; }
        public float Weight { get; set; }
        public string Symbol { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string ImageId { get; set; }
    }
}