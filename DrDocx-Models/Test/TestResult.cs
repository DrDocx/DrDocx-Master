using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DrDocx.Models
{
    public class TestResult
    {
        public int Id { get; set; }
        public double RawScore { get; set; }
        public double StandardizedScore { get; set; }
        public double Percentile { get; set; } // TODO: Calculate automatically based on conversion from standardized score.
        public int TestResultGroupId { get; set; }
        public Patient Patient { get; set; }
        public int PatientId { get; set; }
        public TestResultGroup TestResultGroup { get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; }
    }
}
