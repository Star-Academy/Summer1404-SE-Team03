namespace ScoreManager.Interfaces
{
    public interface IScoreService
    {
        Task<IScore> AddScoreForStudentAsync(int studentNumber, string lesson, double scoreValue);
    }
}