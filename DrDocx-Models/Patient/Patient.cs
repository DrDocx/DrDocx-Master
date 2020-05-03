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
    
    public class Patient : NamedModelBase
    {
        public List<TestResultGroup> ResultGroups { get; set; } = new List<TestResultGroup>();
        public List<FieldValueGroup> FieldValueGroups { get; set; } = new List<FieldValueGroup>();
    }
}