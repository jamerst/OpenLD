using System;
using System.ComponentModel.DataAnnotations;

namespace openld.Models {
    public class FixtureMode {
        public string Id { get; set; }
        public string Name { get; set; }
        public Fixture Fixture { get; set; }
        public string[] Addresses { get; set; }
    }
}