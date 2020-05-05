using System.Collections.Generic;

namespace DrDocx.Models
{
    public class FieldGroup : FieldGroupBase
    { 
        public bool IsDefaultGroup { get; set; }
        public List<Field> Fields { get; set; } = new List<Field>();
        public bool IsArchived { get; set; } = false;
    }
}