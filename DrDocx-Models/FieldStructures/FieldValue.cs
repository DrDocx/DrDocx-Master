namespace DrDocx.Models
{ 
    public class FieldValue
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }
        public int FieldValueGroupId { get; set; }
        public FieldValueGroup FieldValueGroup { get; set; }
        public string FieldTextValue { get; set; }
    }

}