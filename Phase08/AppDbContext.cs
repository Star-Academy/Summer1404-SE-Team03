using Microsoft.EntityFrameworkCore;

namespace Phase01
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Score> Scores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=tempdb;Username=postgres;Password=mypassword");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasKey(s => s.StudentNumber);

            modelBuilder.Entity<Score>()
                .HasOne(s => s.Student)
                .WithMany(st => st.Scores)
                .HasForeignKey(s => s.StudentNumber);
        }
    }
}
