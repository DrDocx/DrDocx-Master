using System;
using System.Linq;

namespace DrDocx.Models.Helpers
{
    public static class TestHelper
    {
        private static readonly string[] ValidScoreTypes =
        {
            "SS (Wechsler)",
            "Scaled Score (Wechsler)",
            "T-Score",
            "z-Score",
            "Percentile",
            "Other"
        };

        public static bool ScoreTypeIsValid(string scoreType) =>
            ValidScoreTypes.Contains(scoreType, StringComparer.OrdinalIgnoreCase);

        public static float GetPercentileForScore(float score, string scoreType)
        {
            return scoreType switch
            {
                "SS (Wechsler)" => GetPercentileForSS(score),
                "Scaled Score (Wechsler)" => GetPercentileForScaledScore(score),
                "T Score" => GetPercentileForTScore(score),
                "z Score" => GetPercentileForZScore(score),
                "Percentile" => score,
                _ => 0
            };
        }

        private static float GetPercentileForZScore(in float score)
        {
            throw new NotImplementedException();
        }

        private static float GetPercentileForTScore(in float score)
        {
            throw new NotImplementedException();
        }

        private static float GetPercentileForScaledScore(in float score)
        {
            throw new NotImplementedException();
        }

        private static float GetPercentileForSS(in float score)
        {
            throw new NotImplementedException();
        }
    }
}