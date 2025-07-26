using System;
using System.Text.Json.Serialization; 

namespace Phase01
{
    public class Score
    {
        public int StudentNumber { get; set; }
        public string? Lesson { get; set; }

        [JsonPropertyName("Score")] // This tells JsonSerializer to map the JSON key "Score" to this property.
        public double Value { get; set; }
    }
}