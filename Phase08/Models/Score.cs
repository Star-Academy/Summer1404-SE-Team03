using ScoreManager.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ScoreManager.Models
{
    public class Score : IScore
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string Lesson { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("Score")]
        public double Value { get; set; }

        [ForeignKey("StudentNumber")]
        public Student Student { get; set; } = null!;

        IStudent IScore.Student
        {
            get => Student;
            set => Student = (Student)value;
        }
    }
}