﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DrDocx.Models
{
    public class TestGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefaultGroup { get; set; }
        [JsonIgnore]
        public List<Test> Tests { get; } = new List<Test>();
    }
}