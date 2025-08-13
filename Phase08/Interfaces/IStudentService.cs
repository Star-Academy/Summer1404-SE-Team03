namespace ScoreManager.Interfaces
{
    public interface IStudentService
    {
        Task<IStudent> AddNewStudentAsync(int studentNumber, string firstName, string lastName);
        Task<IStudent?> FindStudentByNumberAsync(int studentNumber);
        Task<IEnumerable<IStudent>> GetAllStudentsAsync();
    }
}