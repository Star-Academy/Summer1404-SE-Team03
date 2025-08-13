using ScoreManager.DTOs;
using ScoreManager.Interfaces;

namespace ScoreManager.Services
{
    /// <summary>
    /// Implements the logic for generating reports and analytics about students.
    /// </summary>
    public class ReportingService : IReportingService
    {
        private readonly IDbExecuter _dbExecuter;

        // Depends on the IDbExecuter abstraction, not a concrete class (DIP)
        public ReportingService(IDbExecuter dbExecuter)
        {
            _dbExecuter = dbExecuter;
        }

        /// <summary>
        /// This method has a single responsibility: to calculate and return top students. (SRP)
        /// It does not care about how the data is stored or how it will be displayed.
        /// </summary>
        public async Task<IEnumerable<StudentAverageScoreDto>> GetTopStudentsByAverageScoreAsync(int count)
        {
            if (count <= 0)
            {
                return Enumerable.Empty<StudentAverageScoreDto>();
            }

            // 1. Fetch all students with their scores.
            var allStudents = await _dbExecuter.GetAllStudentsWithScoresAsync();

            // 2. Project the data into our DTO, calculating the average score.
            //    This logic is handled in-memory after data retrieval.
            var studentAverages = allStudents
                .Select(student => new StudentAverageScoreDto
                {
                    StudentNumber = student.StudentNumber,
                    FirstName = student.FirstName ?? "N/A",
                    LastName = student.LastName ?? "N/A",
                    // Important: Handle cases where a student has no scores to avoid division by zero.
                    AverageScore = student.Scores.Any() ? student.Scores.Average(s => s.Value) : 0.0
                });

            // 3. Order the results by the average score in descending order and take the top 'count'.
            var topStudents = studentAverages
                .OrderByDescending(s => s.AverageScore)
                .Take(count)
                .ToList();

            return topStudents;
        }
    }
}