using Microsoft.EntityFrameworkCore;
using ScoreManager.Interfaces;
using ScoreManager.Models;

namespace ScoreManager.DataBase
{
    public class ScoreManagerDbExecuter : IDbExecuter
    {
        private readonly IDbContextFactory<ScoreManagerDbContext> _contextFactory;

        public ScoreManagerDbExecuter(IDbContextFactory<ScoreManagerDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IStudent> AddStudentAsync(IStudent student)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var studentEntity = new Student
            {
                StudentNumber = student.StudentNumber,
                FirstName = student.FirstName,
                LastName = student.LastName
            };
            await context.Students.AddAsync(studentEntity);
            await context.SaveChangesAsync();
            return studentEntity;
        }

        public async Task<IScore> AddScoreAsync(IScore score)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var scoreEntity = new Score
            {
                StudentNumber = score.StudentNumber,
                Lesson = score.Lesson,
                Value = score.Value
            };
            await context.Scores.AddAsync(scoreEntity);
            await context.SaveChangesAsync();
            return scoreEntity;
        }

        public async Task<IStudent?> GetStudentByNumberAsync(int studentNumber)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Students
                .Include(s => s.Scores)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StudentNumber == studentNumber);
        }

        public async Task<IEnumerable<IStudent>> GetAllStudentsWithScoresAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Students
                .Include(s => s.Scores)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}