using System.ComponentModel.DataAnnotations.Schema;

namespace openld.Models {
    public class Template {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public Drawing Drawing { get; set; }

    }
}