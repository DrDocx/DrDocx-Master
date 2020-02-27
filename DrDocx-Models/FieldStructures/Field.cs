using System.Text.Json.Serialization;

namespace DrDocx.Models
{ public class Field
    {
        public int Id { get; set; }
        public int FieldGroupId { get; set; }
        [JsonIgnore]
        public FieldGroup FieldGroup { get; set; }
        public string Name { get; set; }
        public string MatchText { get; set; }
        public string DefaultText { get; set; }
        public string Type { get; set; }
    }
}