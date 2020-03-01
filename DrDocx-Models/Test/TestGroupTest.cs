using System.Text.Json.Serialization;

namespace DrDocx.Models
{
    public class TestGroupTest
    {
        public int Id { get; set; }
        [JsonIgnore]
        public Test Test { get; set; }
        public int TestId { get; set; }
        public TestGroup TestGroup { get; set; }
        public int TestGroupId { get; set; }
    }
}