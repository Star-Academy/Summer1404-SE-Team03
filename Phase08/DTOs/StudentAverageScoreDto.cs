namespace ScoreManager.DTOs
{
    public class StudentAverageScoreDto
    {
        public int StudentNumber { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public double AverageScore { get; set; }
    }
}