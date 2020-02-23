using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DrDocx.Models
{

    public class TestResult
    {
        public int Id { get; set; }
        public int RawScore { get; set; }
        public int StandardizedScore { get; set; }
        public int Percentile { get; set; } // TODO: Calculate automatically based on conversion from standardized score.
        public Test RelatedTest { get; set; }
    }
}
