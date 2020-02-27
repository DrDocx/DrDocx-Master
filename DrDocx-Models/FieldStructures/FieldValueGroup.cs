using System.Collections.Generic;

namespace DrDocx.Models
{
    public class FieldValueGroup
    {
        public int Id { get; set; }
        public FieldGroup FieldGroup { get; set; }
        public Patient Patient { get; set; }
        public List<FieldValue> FieldValues { get; } = new List<FieldValue>();
    }
}
