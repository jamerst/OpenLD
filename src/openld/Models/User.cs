using System.ComponentModel.DataAnnotations.Schema;

namespace openld.Models {
    public class User {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
    }
}