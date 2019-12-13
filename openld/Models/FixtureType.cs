using System.ComponentModel.DataAnnotations.Schema;

namespace openld.Models {
    public class FixtureType {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}