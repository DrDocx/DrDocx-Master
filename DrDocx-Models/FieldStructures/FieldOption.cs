using System.Text.Json.Serialization;

namespace DrDocx.Models
{
    public class FieldOption
    {
        public int Id { get; set; }
        [JsonIgnore]
        public Field ParentField { get; set; }
        public int ParentFieldId { get; set; }
        public string Name { get; set; }
    }
}