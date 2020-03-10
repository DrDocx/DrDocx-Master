using System;

namespace DrDocx.Models
{
    public class ReportTemplate : NamedModelBase
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}