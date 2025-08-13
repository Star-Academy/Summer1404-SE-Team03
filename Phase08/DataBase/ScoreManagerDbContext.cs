using Microsoft.EntityFrameworkCore;
using ScoreManager.Models; // Use the concrete Models namespace

namespace ScoreManager.DataBase
{
    public class ScoreManagerDbContext : DbContext
    {
        // DbSets MUST use concrete classes
        public DbSet<Student> Students { get; set; }
        public DbSet<Score> Scores { get; set; }

        public ScoreManagerDbContext(DbContextOptions<ScoreManagerDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configure the Student Entity using the CONCRETE class ---
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentNumber);

                // Define the one-to-many relationship using the concrete types
                // A Student has many Scores.
                entity.HasMany(student => student.Scores)          // Navigation property in Student is 'Scores' (ICollection<Score>)
                      .WithOne(score => score.Student)             // Navigation property in Score is 'Student' (Student)
                      .HasForeignKey(score => score.StudentNumber) // The foreign key in the Score table
                      .IsRequired();                               // A score must belong to a student
            });

            // --- Configure the Score Entity using the CONCRETE class ---
            modelBuilder.Entity<Score>(entity =>
            {
                entity.HasKey(s => s.Id);

                // The other side of the relationship is already configured above.
                // You can add other configurations here if needed.
                entity.Property(s => s.Lesson).IsRequired();
            });
        }
    }
}