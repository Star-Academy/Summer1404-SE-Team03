using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScoreManager.DataBase;
using ScoreManager.Models;
using System.Text.Json;

namespace ScoreManager.Tools
{
    public class DatabaseSeeder
    {
        private readonly IDbContextFactory<ScoreManagerDbContext> _contextFactory;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            IDbContextFactory<ScoreManagerDbContext> contextFactory,
            ILogger<DatabaseSeeder> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task SeedDataAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            if (await context.Students.AnyAsync() || await context.Scores.AnyAsync())
            {
                _logger.LogInformation("Database already contains data. Seeding will be skipped.");
                return;
            }

            _logger.LogInformation("Database is empty. Seeding initial data...");

            await SeedStudentsAsync(context);
            await SeedScoresAsync(context);

            await context.SaveChangesAsync();
            _logger.LogInformation("Database seeding complete.");
        }

        private async Task SeedStudentsAsync(ScoreManagerDbContext context)
        {
            const string studentFilePath = "students.json";
            if (!File.Exists(studentFilePath))
            {
                _logger.LogWarning("Student data file not found at '{FilePath}'. Skipping student seeding.", studentFilePath);
                return;
            }

            var studentJson = await File.ReadAllTextAsync(studentFilePath);
            var students = JsonSerializer.Deserialize<List<Student>>(studentJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (students != null && students.Any())
            {
                await context.Students.AddRangeAsync(students);
                _logger.LogInformation("  -> Loaded {StudentCount} students.", students.Count);
            }
        }

        private async Task SeedScoresAsync(ScoreManagerDbContext context)
        {
            const string scoreFilePath = "scores.json";
             if (!File.Exists(scoreFilePath))
            {
                _logger.LogWarning("Score data file not found at '{FilePath}'. Skipping score seeding.", scoreFilePath);
                return;
            }

            var scoreJson = await File.ReadAllTextAsync(scoreFilePath);
            var scores = JsonSerializer.Deserialize<List<Score>>(scoreJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (scores != null && scores.Any())
            {
                await context.Scores.AddRangeAsync(scores);
                _logger.LogInformation("  -> Loaded {ScoreCount} scores.", scores.Count);
            }
        }
    }
}