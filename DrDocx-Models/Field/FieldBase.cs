using System.Text.Json.Serialization;

namespace DrDocx.Models
{
    public abstract class FieldBase : NamedModelBase
    {
        public string MatchText { get; set; }
        public string Type { get; set; }
    }
}