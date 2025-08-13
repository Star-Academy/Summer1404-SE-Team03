using Microsoft.EntityFrameworkCore;
using ScoreManager.Models;

namespace ScoreManager.DataBase
{
    public class ScoreManagerDbContext : DbContext
    {
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Score> Scores { get; set; }

        public ScoreManagerDbContext(DbContextOptions<ScoreManagerDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentNumber);
                entity.HasMany(student => student.Scores)
                      .WithOne(score => score.Student)
                      .HasForeignKey(score => score.StudentNumber)
                      .IsRequired();
            });

            modelBuilder.Entity<Score>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Lesson).IsRequired();
            });
        }
    }
}