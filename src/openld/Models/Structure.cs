using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace openld.Models {
    public class Structure {
        public string Id { get; set; }
        public View View { get; set; }
        public Geometry Geo { get; set; }
        public List<RiggedFixture> Fixtures { get; set; }
        public string Name { get; set; }
        public float rating { get; set; }
        public StructureType Type { get; set; }
        public string Notes { get; set; }
    }
}