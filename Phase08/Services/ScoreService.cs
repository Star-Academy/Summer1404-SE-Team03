using ScoreManager.Interfaces;
using ScoreManager.Models;

namespace ScoreManager.Services
{
    public class ScoreService : IScoreService
    {
        private readonly IDbExecuter _dbExecuter;

        public ScoreService(IDbExecuter dbExecuter)
        {
            _dbExecuter = dbExecuter;
        }

        public async Task<IScore> AddScoreForStudentAsync(int studentNumber, string lesson, double scoreValue)
        {
            // Business logic: check if student exists before adding a score
            var student = await _dbExecuter.GetStudentByNumberAsync(studentNumber);
            if (student == null)
            {
                throw new KeyNotFoundException($"Student with number {studentNumber} not found.");
            }

            if (scoreValue < 0 || scoreValue > 20)
            {
                throw new ArgumentOutOfRangeException(nameof(scoreValue), "Score must be between 0 and 20.");
            }

            var newScore = new Score
            {
                StudentNumber = studentNumber,
                Lesson = lesson,
                Value = scoreValue
            };

            return await _dbExecuter.AddScoreAsync(newScore);
        }
    }
}