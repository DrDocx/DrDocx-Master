using System.Text.Json.Serialization;

namespace DrDocx.Models
{
    public class FieldOptionValue : NamedModelBase
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        public FieldOptionValue(FieldOption fieldOption, FieldValue parent)
        {
            Name = fieldOption.Name;
            ParentFieldValue = parent;
        }

        [JsonIgnore]
        public FieldValue ParentFieldValue { get; set; }
        public int ParentFieldValueId { get; set; }
        public bool IsSelected { get; set; } = false;
    }
}