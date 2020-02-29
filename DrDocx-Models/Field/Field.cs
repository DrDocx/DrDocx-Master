using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DrDocx.Models
{
    public class Field : FieldBase
    {
        [JsonIgnore]
        public FieldGroup FieldGroup { get; set; }
        public int FieldGroupId { get; set; }
        public string DefaultText { get; set; }
        
        public List<FieldOption> FieldOptions { get; } = new List<FieldOption>();
    }

    public enum FieldType
    {
        [Display(Name = "Text")]
        SmallText,
        [Display(Name = "Paragraph")]
        BigText,
        [Display(Name = "Date/Time")]
        Date,
        [Display(Name = "Select One")]
        SelectOne,
        [Display(Name = "Select Multiple")]
        SelectMultiple
    }
}