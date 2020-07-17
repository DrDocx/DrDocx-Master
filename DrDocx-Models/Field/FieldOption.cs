using System.Text.Json.Serialization;

namespace DrDocx.Models
{
    public class FieldOption : NamedModelBase
    {
        [JsonIgnore]
        public Field ParentField { get; set; }
        public int ParentFieldId { get; set; }
    }
}