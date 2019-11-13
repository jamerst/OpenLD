using System.Collections.Generic;

namespace openld.Models {
    public class View {
        public string Id { get; set; }
        public Drawing Drawing { get; set; }
        public string Name { get; set; }
        public List<Structure> Structures { get; set; }
        public int Type { get; set; }
    }
}