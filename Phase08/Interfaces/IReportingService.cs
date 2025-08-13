using ScoreManager.DTOs;

namespace ScoreManager.Interfaces
{
    /// <summary>
    /// Defines a contract for services that provide reporting and analytics.
    /// </summary>
    public interface IReportingService
    {
        /// <summary>
        /// Gets a specified number of students with the highest average scores.
        /// </summary>
        /// <param name="count">The number of top students to return.</param>
        /// <returns>A collection of DTOs representing the top students and their average scores.</returns>
        Task<IEnumerable<StudentAverageScoreDto>> GetTopStudentsByAverageScoreAsync(int count);
    }
}