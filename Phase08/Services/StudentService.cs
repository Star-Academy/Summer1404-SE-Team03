using ScoreManager.Interfaces;
using ScoreManager.Models;

namespace ScoreManager.Services
{
    public class StudentService : IStudentService
    {
        private readonly IDbExecuter _dbExecuter;

        public StudentService(IDbExecuter dbExecuter)
        {
            _dbExecuter = dbExecuter;
        }

        public async Task<IStudent> AddNewStudentAsync(int studentNumber, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("First and last names cannot be empty.");
            }

            var newStudent = new Student
            {
                StudentNumber = studentNumber,
                FirstName = firstName,
                LastName = lastName
            };

            return await _dbExecuter.AddStudentAsync(newStudent);
        }

        public async Task<IStudent?> FindStudentByNumberAsync(int studentNumber)
        {
            return await _dbExecuter.GetStudentByNumberAsync(studentNumber);
        }

        public async Task<IEnumerable<IStudent>> GetAllStudentsAsync()
        {
            return await _dbExecuter.GetAllStudentsWithScoresAsync();
        }
    }
}