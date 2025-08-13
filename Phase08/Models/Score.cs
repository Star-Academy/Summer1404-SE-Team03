using ScoreManager.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public double Value { get; set; }

        // --- CORRECTION ---
        // For EF Core's relationship mapping, use the concrete Student type.
        [ForeignKey("StudentNumber")]
        public Student Student { get; set; } = null!;

        // This explicitly implements the interface property.
        // The rest of your app sees IStudent, but EF sees the concrete Student.
        IStudent IScore.Student
        {
            get => Student;
            set => Student = (Student)value;
        }
    }
}