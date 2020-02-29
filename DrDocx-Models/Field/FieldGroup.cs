using System.Collections.Generic;

namespace DrDocx.Models
{
    public class FieldGroup : FieldGroupBase
    { 
        public List<Field> Fields { get; } = new List<Field>();
    }
}