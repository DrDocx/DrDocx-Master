namespace DrDocx.Models
{ 
    public class FieldValue
    {
        public FieldValue(Field field, FieldValueGroup fieldValueGroup)
        {
            Field = field;
            FieldValueGroup = fieldValueGroup;
        }

        public int Id { get; set; }
        public Field Field { get; set; }
        public FieldValueGroup FieldValueGroup { get; set; }
        public string FieldTextValue { get; set; }
    }

}