namespace DrDocx.Models
{
    public class FieldOptionValue
    {
        public int Id { get; set; }
        public FieldOption FieldOption { get; set; }
        public bool IsSelected { get; set; }
    }
}