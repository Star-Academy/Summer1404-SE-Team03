using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ScoreManager.DataBase
{
    public class ScoreManagerDbContextFactory : IDesignTimeDbContextFactory<ScoreManagerDbContext>
    {
        public ScoreManagerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ScoreManagerDbContext>();
            
            const string connectionString = "Host=localhost;Database=score_manager_db;Username=postgres;Password=mypassword";

            optionsBuilder.UseNpgsql(connectionString);

            return new ScoreManagerDbContext(optionsBuilder.Options);
        }
    }
}