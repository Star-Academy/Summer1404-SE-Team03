namespace ScoreManager.Interfaces
{
    public interface IDbExecuter
    {
        Task<IStudent> AddStudentAsync(IStudent student);
        Task<IScore> AddScoreAsync(IScore score);
        Task<IStudent?> GetStudentByNumberAsync(int studentNumber);
        Task<IEnumerable<IStudent>> GetAllStudentsWithScoresAsync();
    }
}