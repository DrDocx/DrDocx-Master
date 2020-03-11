using System.Collections.Generic;
using NUnit.Framework;
using DrDocx.Models.Helpers;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

// ReSharper disable InconsistentNaming

namespace DrDocx.Tests.Models
{
    [TestFixture]
    public class TestScoringTests
    {
        private List<ScoreEquivalence> ScoreEquivalences;

        [OneTimeSetUp]
        public void Init()
        {
            var scoreJson = File.ReadAllText("ScoreTable.json");
            ScoreEquivalences = JsonConvert.DeserializeObject<List<ScoreEquivalence>>(scoreJson);
        }

        [TearDown]
        public void Cleanup()
        {
            
        }

        [Test]
        public void PercentileIsCorrectForZScore()
        {
            foreach (var scoreEquivalence in ScoreEquivalences)
            {
                var percentileEstimate = TestHelper.GetPercentileForScore(scoreEquivalence.ZScore, "z Score");
                Assert.AreEqual(scoreEquivalence.Percentile, percentileEstimate, 0.01);
            }
        }

        [Test]
        public void PercentileIsCorrectForSS()
        {
            foreach (var scoreEquivalence in ScoreEquivalences)
            {
                var percentileEstimate = TestHelper.GetPercentileForScore(scoreEquivalence.SS, "SS");
                Assert.AreEqual(scoreEquivalence.Percentile, percentileEstimate, 2);
            }
        }

        [Test]
        public void PercentileIsCorrectForTScore()
        {
            foreach (var scoreEquivalence in ScoreEquivalences)
            {
                var percentileEstimate = TestHelper.GetPercentileForScore(scoreEquivalence.TScore, "T Score");
                Assert.AreEqual(scoreEquivalence.Percentile, percentileEstimate, 3);
            }
        }

        [Test]
        public void PercentileIsCorrectForScaledScore()
        {
            
        }
        
    }

    internal class ScoreEquivalence
    {
        [JsonProperty("Scaled Score")]
        public double ScaledScore { get; set; }
        [JsonProperty("SS")]
        public double SS { get; set; }
        [JsonProperty("T")]
        public double TScore { get; set; }
        [JsonProperty("z")]
        public double ZScore { get; set; }
        [JsonProperty("Percentile")]
        public double Percentile { get; set; }
    } 
}