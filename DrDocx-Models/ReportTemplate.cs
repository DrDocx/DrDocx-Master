using System;

namespace DrDocx.Models
{
    public class ReportTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public DateTime DateCreated { get; set; }
    }
}