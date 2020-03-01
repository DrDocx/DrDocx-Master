using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Reflection;

namespace DrDocx.Models
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public StandardizedScoreType StandardizedScoreType { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public List<TestGroupTest> TestGroupTests { get; } = new List<TestGroupTest>();
    }

    /// <summary>
    /// Note: These 
    /// </summary>
    public enum StandardizedScoreType
    {
        [Display(Name = "SS")]
        Ss,
        [Display(Name = "Scaled Score")]
        ScaledScore,
        [Display(Name = "Z Score")]
        ZScore,
        Percentile
    }
}