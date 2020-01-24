using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace openld.Models {
    public class Structure {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public View View { get; set; }
        [Column(TypeName = "jsonb")]
        public Geometry Geometry { get; set; }
        public List<RiggedFixture> Fixtures { get; set; } = new List<RiggedFixture>();
        public string Name { get; set; }
        public float Rating { get; set; }
        public StructureType Type { get; set; }
        public string Notes { get; set; } = "";
    }
}