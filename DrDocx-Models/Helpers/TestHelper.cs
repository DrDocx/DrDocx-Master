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
            "T-Score (Wechsler)",
            "Z-Score",
            "Percentile",
            "Other"
        };

        public static bool ScoreTypeIsValid(string scoreType) =>
            ValidScoreTypes.Contains(scoreType, StringComparer.OrdinalIgnoreCase);
    }
}