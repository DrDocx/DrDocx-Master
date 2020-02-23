using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DrDocx.Models
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StandardizedScoreType { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public List<TestGroupTest> TestGroupTests { get; set; }
    }
    
    // ReSharper disable once InconsistentNaming
    public enum ScoreTypes {SS, StandardScore, }
}