using ScoreManager.DTOs;

namespace ScoreManager.Interfaces
{
    public interface IReportingService
    {
        Task<IEnumerable<StudentAverageScoreDto>> GetTopStudentsByAverageScoreAsync(int count);
    }
}