using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace openld.Models {
    public class Drawing {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Title { get; set; }
        public User Owner { get; set; }
        public DateTime LastModified { get; set; }
        public List<View> Views { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
}