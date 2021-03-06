﻿using System.Collections.Generic;
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
        public ScoreType StandardizedScoreType { get; set; }
        public ScoreType StandardizedScoreTypeId { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public TestGroup TestGroup { get; set; }
        public int TestGroupId { get; set; }
    }
}