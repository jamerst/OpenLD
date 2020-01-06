using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace openld.Models {
    public class View {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public Drawing Drawing { get; set; }
        public string Name { get; set; }
        public List<Structure> Structures { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int Type { get; set; }
    }
}