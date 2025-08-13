using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ScoreManager.DataBase
{
    // This factory is primarily for design-time tools (e.g., migrations)
    public class ScoreManagerDbContextFactory : IDesignTimeDbContextFactory<ScoreManagerDbContext>
    {
        public ScoreManagerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ScoreManagerDbContext>();
            
            // Replace with your actual connection string, perhaps from a config file
            const string connectionString = "Host=localhost;Database=score_manager_db;Username=postgres;Password=mypassword";

            optionsBuilder.UseNpgsql(connectionString);

            return new ScoreManagerDbContext(optionsBuilder.Options);
        }
    }
}