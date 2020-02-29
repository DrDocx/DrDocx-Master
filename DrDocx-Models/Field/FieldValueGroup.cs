using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DrDocx.Models
{
    public class FieldValueGroup : FieldGroupBase
    {
        public FieldGroup FieldGroup { get; set; }
        [JsonIgnore]
        public Patient Patient { get; set; }
        public int PatientId { get; set; }
        public List<FieldValue> FieldValues { get; } = new List<FieldValue>();
    }
}
