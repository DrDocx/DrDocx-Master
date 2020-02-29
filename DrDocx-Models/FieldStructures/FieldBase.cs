using System.Text.Json.Serialization;

namespace DrDocx.Models
{
    public abstract class FieldBase
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string MatchText { get; set; }
        public FieldType Type { get; set; }
    }
}