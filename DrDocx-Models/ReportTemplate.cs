using System;

namespace DrDocx.Models
{
    public class ReportTemplate
    {
        public int Id { get; set; }
        public int Name { get; set; }
        public string FilePath { get; set; }
        public DateTime DateCreated { get; set; }
    }
}