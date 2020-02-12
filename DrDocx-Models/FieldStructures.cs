using System;
using System.Collections.Generic;
using System.Text;

namespace DrDocx.Models
{
    public class Field
    {
        public int Id { get; set; }
        public int FieldGroupId { get; set; }
        public string Name { get; set; }
        public string MatchText { get; set; }
        public string DefaultText { get; set; }
        public string Type { get; set; }
    }

    public class FieldGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Field> Fields { get; set; }
    }

    public class FieldValue
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public int FieldValueGroupId { get; set; }
        public string FieldTextValue { get; set; }
    }

    public class FieldValueGroup
    {
        public int Id { get; set; }
        public int FieldGroupId { get; set; }
        public int PatientId { get; set; }
        public List<FieldValue> FieldValues { get; set; }
    }
}
