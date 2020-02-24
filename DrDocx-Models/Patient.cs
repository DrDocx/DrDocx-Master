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

    /// <summary>
    ///  Used to send basic info when a list of patients are sent and not all info is needed.
    /// </summary>
    public class PatientInfo
    {
        public string Name { get; set; }
        public DateTime DateModified { get; set; }
        public int Id { get; set; }
    }
}