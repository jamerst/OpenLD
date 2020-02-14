using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace openld.Models {
    public class View {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public Drawing Drawing { get; set; }
        public string Name { get; set; }
        public List<Structure> Structures { get; set; }
        public List<Label> Labels { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int Type { get; set; }
        public View Clone() {
            View view = new View {
                Id = null,
                Drawing = null,
                Name = this.Name,
                Structures = new List<Structure>(),
                Labels = new List<Label>(),
                Width = this.Width,
                Height = this.Height,
                Type = this.Type
            };

            foreach(Structure s in this.Structures) {
                view.Structures.Add(s.Clone());
            }

            foreach(Label l in this.Labels) {
                view.Labels.Add(l.clone());
            }

            return view;
        }
    }
}