using System.Collections.Generic;

namespace DrDocx.Models
{
    public class TestResultGroup
    {
        public int Id { get; set; }
        public TestGroup TestGroupInfo { get; set; }
        public List<TestResult> Tests { get; set; } = new List<TestResult>();
        public Patient Patient { get; set; }
        public int PatientId { get; set; }
    }
}