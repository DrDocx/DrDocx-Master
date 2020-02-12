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
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public List<TestGroupTest> TestGroupTests { get; set; }
        public int Id { get; set; }

        /*public Test(string name, string description)
        {
            Name = name;
            Description = description;
        }*/
    }

    public class TestGroup
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public List<TestGroupTest> TestGroupTests { get; set; }

        public int Id { get; set; }

         /*This can be used for when a test group doesn't have tests ready to populate it with
        public TestGroup(string name, string description, int id)
        {
            Name = name;
            Description = description;
            Tests = null;
            Id = id;
        }

        public TestGroup(string name, string description, List<DTest> tests, int id)
        {
            Name = name;
            Description = description;
            Tests = tests;
            Id = id;
        }*/
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
        public TestGroup TestGroupInfo { get; set; }
        public List<TestResult> Tests { get; set; }

        public int Id { get; set; }
    }

    public class TestResult
    {
        public int RawScore { get; set; }
        public int ScaledScore { get; set; }
        public int ZScore { get; set; }
        public int Percentile { get; set; }
        public Test RelatedTest { get; set; }

        public int Id { get; set; }
    }
}
