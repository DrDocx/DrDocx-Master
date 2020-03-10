using System;
using System.Linq;

namespace DrDocx.Models.Helpers
{
    public static class TestHelper
    {
        private static readonly string[] ValidScoreTypes =
        {
            "SS",
            "Scaled Score",
            "T Score",
            "z Score",
            "Percentile",
            "Other"
        };

        public static bool ScoreTypeIsValid(string scoreType) =>
            ValidScoreTypes.Contains(scoreType, StringComparer.OrdinalIgnoreCase);

        public static float GetPercentileForScore(float score, string scoreType)
        {
            return scoreType switch
            {
                "SS" => GetPercentileForSS(score),
                "Scaled Score" => GetPercentileForScaledScore(score),
                "T Score" => GetPercentileForTScore(score),
                "z Score" => GetPercentileForZScore(score),
                "Percentile" => score,
                _ => -1
            };
        }

        private static float GetPercentileForZScore(in float score)
        {
            return GaussianCDF(score) * 100; // Convert from probability to percentile
        }

        private static float GetPercentileForTScore(in float score)
        {
            var zScore = (score - 50) / 10;
            return GetPercentileForZScore(zScore);
        }

        private static float GetPercentileForScaledScore(in float score)
        {
            throw new NotImplementedException();
        }

        private static float GetPercentileForSS(in float score)
        {
            throw new NotImplementedException();
        }
        
        // Credit: https://www.johndcook.com/blog/csharp_phi/
        private static float GaussianCDF(float zScore)
        {
            double x = zScore;
            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;
        
            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x) / Math.Sqrt(2.0);
        
            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p*x);
            double y = 1.0 - (((((a5*t + a4)*t) + a3)*t + a2)*t + a1)*t * Math.Exp(-x*x);
        
            return (float) (0.5 * (1.0 + sign*y));
        }
    }
}