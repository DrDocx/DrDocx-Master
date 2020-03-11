using System;
using System.Collections.Generic;
using NUnit.Framework;
using DrDocx.Models.Helpers;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

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

        [Test]
        public void PercentileIsCorrectForZScore()
        {
            foreach (var scoreEquivalence in ScoreEquivalences)
            {
                var percentileEstimate = TestHelper.GetPercentileForScore(scoreEquivalence.ZScore, "z Score");
                Assert.AreEqual(scoreEquivalence.Percentile, percentileEstimate, delta: 0.01);
            }
        }

        [Test]
        public void PercentileIsCorrectForSS()
        {
            foreach (var scoreEquivalence in ScoreEquivalences)
            {
                var percentileEstimate = TestHelper.GetPercentileForScore(scoreEquivalence.SS, "SS");
                Assert.AreEqual(scoreEquivalence.Percentile, percentileEstimate, delta: 2);
            }
        }

        [Test]
        public void PercentileIsCorrectForTScore()
        {
            foreach (var scoreEquivalence in ScoreEquivalences)
            {
                var percentileEstimate = TestHelper.GetPercentileForScore(scoreEquivalence.TScore, "T Score");
                Assert.AreEqual(scoreEquivalence.Percentile, percentileEstimate, delta: 3);
            }
        }

        [Test]
        public void PercentileIsCorrectForScaledScore()
        {
            // We test the average case of each scaled score since the range is so small (1 - 19)
            for (var i = 1; i <= 19; i++)
            {
                var scaledScore = i;
                var equivalencesForScaledScore = ScoreEquivalences.Where(se => (int) se.ScaledScore - scaledScore == 0).ToList();
                if (!equivalencesForScaledScore.Any()) continue;
                var middleScoreEquivalence = equivalencesForScaledScore[equivalencesForScaledScore.Count() / 2];
                var percentileEstimate = TestHelper.GetPercentileForScore(middleScoreEquivalence.ScaledScore, "Scaled Score");
                Assert.AreEqual(middleScoreEquivalence.Percentile, percentileEstimate, delta: 4);
            }
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