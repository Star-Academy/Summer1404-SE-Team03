using ScoreManager.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ScoreManager.Models
{
    public class Student : IStudent
    {
        [Key]
        public int StudentNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        public ICollection<Score> Scores { get; set; } = new List<Score>();

        ICollection<IScore> IStudent.Scores
        {
            get => Scores.Cast<IScore>().ToList();
            set => Scores = value.Cast<Score>().ToList();
        }
    }
}