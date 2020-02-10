using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

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
        public Structure Clone() {
            Structure structure = new Structure {
                Id = null,
                View = null,
                Geometry = this.Geometry,
                Fixtures = new List<RiggedFixture>(),
                Name = this.Name,
                Rating = this.Rating,
                Type = this.Type,
                Notes = this.Notes
            };

            foreach(RiggedFixture f in this.Fixtures) {
                structure.Fixtures.Add(f.Clone());
            }

            return structure;
        }

        public object this[string propName] {
            get {
                Type t = typeof(Structure);
                PropertyInfo propInfo = t.GetProperty(propName, BindingFlags.IgnoreCase |  BindingFlags.Public | BindingFlags.Instance);
                if (propInfo == null) {
                    throw new KeyNotFoundException("Property not found");
                }
                return propInfo.GetValue(this, null);
            }
        }
    }
}