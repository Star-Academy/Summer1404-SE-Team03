using System.Text.Json.Serialization;

namespace Phase01
{
    public class Score
    {
        public int Id { get; set; }
        public int StudentNumber { get; set; }
        public string Lesson { get; set; } = string.Empty;

        [JsonPropertyName("Score")]
        public double Value { get; set; }

        public Student? Student { get; set; }
    }
}
