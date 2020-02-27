using System.Collections.Generic;

namespace DrDocx.Models
{
    public class FieldGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Field> Fields { get; } = new List<Field>();
    }
}