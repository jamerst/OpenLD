using System.ComponentModel.DataAnnotations.Schema;

namespace openld.Models {
    public class UserDrawing {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public User User { get; set; }
        public Drawing Drawing { get; set; }
    }
}