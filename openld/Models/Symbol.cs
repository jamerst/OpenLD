using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace openld.Models {
    public class Symbol {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Hash { get; set; }
        [JsonIgnore]
        public string Path { get; set; }
        public StoredImage Bitmap { get; set; }
    }
}