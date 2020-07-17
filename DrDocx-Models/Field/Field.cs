using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DrDocx.Models
{
    public class Field : NamedModelBase
    {
        [JsonIgnore]
        public FieldGroup FieldGroup { get; set; }
        public int FieldGroupId { get; set; }
        public string DefaultValue { get; set; } = "";
        public bool IsArchived { get; set; } = false;
        public string MatchText { get; set; }
        public string Type { get; set; }
    }

    public enum FieldType
    {
        [Display(Name = "Text")]
        SmallText,
        [Display(Name = "Paragraph")]
        LargeText,
        [Display(Name = "Date/Time")]
        Date,
        // [Display(Name = "Select One")]
        // SelectOne,
        // [Display(Name = "Select Multiple")]
        // SelectMultiple
    }
}