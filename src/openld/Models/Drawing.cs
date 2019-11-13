using System;
using System.Collections.Generic;

namespace openld.Models {
    public class Drawing {
        public string Id { get; set; }
        public string Title { get; set; }
        public User Owner { get; set; }
        public DateTime LastModified { get; set; }
        public List<View> Views { get; set; }
    }
}