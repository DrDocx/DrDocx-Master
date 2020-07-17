using System.Collections.Generic;

namespace DrDocx.Models
{
    public class FieldGroup : NamedModelBase
    { 
        public string Description { get; set; }
        public bool IsDefaultGroup { get; set; }
        public List<Field> Fields { get; set; } = new List<Field>();
        public bool IsArchived { get; set; } = false;
    }
}