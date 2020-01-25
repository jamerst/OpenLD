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
        public List<UserDrawing> UserDrawings { get; set; }

        public Drawing Clone() {
            Drawing drawing = new Drawing {
                Id = null,
                Title = this.Title,
                Owner = null,
                LastModified = this.LastModified,
                Views = new List<View>(),
                UserDrawings = null
            };

            foreach (View v in this.Views) {
                drawing.Views.Add(v.Clone());
            }

            return drawing;
        }
    }
}