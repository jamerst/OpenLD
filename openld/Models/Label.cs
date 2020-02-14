using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace openld.Models {
    public class Label {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public View View { get; set; }
        [Column(TypeName = "jsonb")]
        public Point Position { get; set; }
        public string Text { get; set; }
        public Label clone() {
            return new Label {
                Id = null,
                Position = this.Position,
                Text = this.Text
            };
        }

        public object this[string propName] {
            get {
                Type t = typeof(Label);
                PropertyInfo propInfo = t.GetProperty(propName, BindingFlags.IgnoreCase |  BindingFlags.Public | BindingFlags.Instance);
                if (propInfo == null) {
                    throw new KeyNotFoundException("Property not found");
                }
                return propInfo.GetValue(this, null);
            }
        }
    }
}