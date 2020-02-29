using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DrDocx.Models
{ 
    public class Field
    {
        public int Id { get; set; }
        public int FieldGroupId { get; set; }
        [JsonIgnore]
        public FieldGroup FieldGroup { get; set; }
        public string Name { get; set; }
        public string MatchText { get; set; }
        public string DefaultText { get; set; }
        
        [JsonIgnore]
        public bool IsRemoved { get; set; }
        public FieldType Type { get; set; }
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