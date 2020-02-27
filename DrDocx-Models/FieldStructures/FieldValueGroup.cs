using System.Collections.Generic;

namespace DrDocx.Models
{
    public class FieldValueGroup
    {
        public int Id { get; set; }
        public FieldGroup FieldGroup { get; set; }
        public Patient Patient { get; set; }
        public List<FieldValue> FieldValues { get; set; }

        public FieldValueGroup(FieldGroup fieldGroup)
        {
            FieldGroup = fieldGroup;
            FieldValues = new List<FieldValue>();
            fieldGroup.Fields.ForEach(field => FieldValues.Add(new FieldValue(field, this)));
        }
    }
}
