using System;
using System.Text.Json.Serialization; 

namespace Phase01
{
    public class Score
    {
        public int StudentNumber { get; set; }
        public string? Lesson { get; set; }

        [JsonPropertyName("Score")]
        public double Value { get; set; }
    }
}