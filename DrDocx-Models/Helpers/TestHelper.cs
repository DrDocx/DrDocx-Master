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

        public static double GetPercentileForScore(double score, string scoreType)
        {
            return scoreType switch
            {
                "SS" => GetPercentileForSS(score),
                "Scaled Score" => GetPercentileForScaledScore(score),
                "T Score" => GetPercentileForTScore(score),
                "z Score" => GetPercentileForZScore(score),
                "Percentile" => score,
                _ => 0
            };
        }

        private static double GetPercentileForZScore(in double score)
        {
            return GetGaussianCdf(score) * 100; // Convert from probability to percentile
        }

        private static double GetPercentileForTScore(in double score)
        {
            // Conversion formula from https://link.springer.com/referenceworkentry/10.1007%2F978-0-387-79948-3_1254
            var zScore = (score - 50) / 10;
            return GetPercentileForZScore(zScore);
        }

        /* 
         * The following SS and Scaled Score conversions are not sourced; rather, due to the observation
         * that they are just differently scaled normal distributions, I ran a linear regression on the provided
         * conversion table in MATLAB to create a formula for a conversion to a z-score. The coefficients are
         * part of the linear equation z = a * score + b.
         */
        private static double GetPercentileForScaledScore(in double score)
        {
            // r^2 = 0.9954
            const double a = 0.342; const double b = -3.415;
            var zScore = a * score + b;
            return GetPercentileForZScore(zScore);
        }

        private static double GetPercentileForSS(in double score)
        {
            // r^2 = 1.0 (not a typo)
            const double a = 0.06667; const double b = -6.666;
            var zScore = a * score + b;
            return GetPercentileForZScore(zScore);
        }
        
        // Credit: https://www.johndcook.com/blog/csharp_phi/
        private static double GetGaussianCdf(double x)
        {
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
        
            return (double) (0.5 * (1.0 + sign*y));
        }
    }
}