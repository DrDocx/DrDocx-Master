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
    /// THIS WILL NOT WORK TIL EVERYTHINGS IN A PROJECT BECAUSE C# IS SPECIAL 
    public class Patient
    {
        public List<TestResultGroup> ResultGroups { get; set; }
        public string Diagnosis { get; set; }
        public string Name { get; set; }
        public string PreferredName { get; set; }
        public string Address { get; set; }
        public string Medications { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfTesting { get; set; }
        public string Notes { get; set; }
        public int MedicalRecordNumber { get; set; }
        public int AgeAtTesting { get; set; }
        
        public List<FieldValueGroup> FieldValueGroups { get; set; }
        public int Id { get; set; }
    }
}