using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace openld.Models {
    public class StoredImage {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [JsonIgnore]
        public string Hash { get; set; }
        [JsonIgnore]
        public string Path { get; set; }
    }
}