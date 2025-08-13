using ScoreManager.DTOs;
using ScoreManager.Interfaces;

namespace ScoreManager.Services
{
    public class ReportingService : IReportingService
    {
        private readonly IDbExecuter _dbExecuter;

        public ReportingService(IDbExecuter dbExecuter)
        {
            _dbExecuter = dbExecuter;
        }

        public async Task<IEnumerable<StudentAverageScoreDto>> GetTopStudentsByAverageScoreAsync(int count)
        {
            if (count <= 0)
            {
                return Enumerable.Empty<StudentAverageScoreDto>();
            }

            var allStudents = await _dbExecuter.GetAllStudentsWithScoresAsync();

            var studentAverages = allStudents
                .Select(student => new StudentAverageScoreDto
                {
                    StudentNumber = student.StudentNumber,
                    FirstName = student.FirstName ?? "N/A",
                    LastName = student.LastName ?? "N/A",
                    AverageScore = student.Scores.Any() ? student.Scores.Average(s => s.Value) : 0.0
                });

            var topStudents = studentAverages
                .OrderByDescending(s => s.AverageScore)
                .Take(count)
                .ToList();

            return topStudents;
        }
    }
}