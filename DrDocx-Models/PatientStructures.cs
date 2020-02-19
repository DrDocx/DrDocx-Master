using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DrDocx.Models
{

    /// <summary>
    /// Summary description for Patient
    /// </summary>
    ///
    
    public class Patient
    {
        public string Name { get; set; }
        public DateTime DateModified { get; set; }
        public List<TestResultGroup> ResultGroups { get; set; }
        public List<FieldValueGroup> FieldValueGroups { get; set; }
        public int Id { get; set; }
    }
}