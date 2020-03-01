using System.Collections.Generic;

namespace DrDocx.Models
{ 
    public class FieldValue : DatabaseModelBase
    {
        public int ParentGroupId { get; set; }
        public FieldValueGroup ParentGroup { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }
        public string FieldTextValue { get; set; }
    }

}