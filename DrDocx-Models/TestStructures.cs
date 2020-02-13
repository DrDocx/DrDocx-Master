using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DrDocx.Models
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public List<TestGroupTest> TestGroupTests { get; set; }
    }

    public class TestGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public List<TestGroupTest> TestGroupTests { get; set; }
    }

    public class TestGroupTest
    {
        public int Id { get; set; }
        [JsonIgnore]
        public Test Test { get; set; }
        public int TestId { get; set; }
        public TestGroup TestGroup { get; set; }
        public int TestGroupId { get; set; }
    }
    
    public class TestResultGroup
    {
        public int Id { get; set; }
        public TestGroup TestGroupInfo { get; set; }
        public List<TestResult> Tests { get; set; }
    }

    public class TestResult
    {
        public int Id { get; set; }
        public int RawScore { get; set; }
        public int StandardizedScore { get; set; }
        public int ZScore { get; set; }
        public int Percentile { get; set; } //TODO: Calculate automatically based on conversion from standardized score.
        public Test RelatedTest { get; set; }
    }
}
