using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DrDocx.Models
{ 
    public class FieldValue : DatabaseModelBase
    {
        public int ParentGroupId { get; set; }
        [JsonIgnore]
        public FieldValueGroup ParentGroup { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }
        public string FieldTextValue { get; set; }
    }

}