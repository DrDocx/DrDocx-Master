using System.Collections.Generic;

namespace DrDocx.Models
{ 
    public class FieldValue : FieldBase
    {
        public int FieldValueGroupId { get; set; }
        public FieldValueGroup FieldValueGroup { get; set; }
        public string FieldTextValue { get; set; }
        public List<FieldOptionValue> FieldOptionValues { get; } = new List<FieldOptionValue>();

        public FieldValue(Field field)
        {
            Name = field.Name;
            Type = field.Type;
            MatchText = field.MatchText;
            FieldTextValue = field.DefaultText;
            field.FieldOptions.ForEach(fo => FieldOptionValues.Add(new FieldOptionValue(fo)));
        }
    }

}