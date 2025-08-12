using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Phase01
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new AppDbContext();
            db.Database.EnsureCreated();

            var studentsJson = File.ReadAllText("students.json");
            var scoresJson = File.ReadAllText("scores.json");

            var students = JsonSerializer.Deserialize<List<Student>>(studentsJson);
            var scores = JsonSerializer.Deserialize<List<Score>>(scoresJson);

            if (students == null || scores == null)
            {
                Console.WriteLine("Error loading JSON files.");
                return;
            }

            if (!db.Students.Any())
            {
                db.Students.AddRange(students);
                db.Scores.AddRange(scores);
                db.SaveChanges();
                Console.WriteLine("Data imported successfully!");
            }

            var topThreeStudents = db.Students
                .Select(s => new
                {
                    Student = s,
                    AverageScore = s.Scores.Average(sc => sc.Value)
                })
                .OrderByDescending(x => x.AverageScore)
                .Take(3)
                .ToList();

            Console.WriteLine("Top Three Students (by Average Score):");
            Console.WriteLine("----------------------------------");
            foreach (var s in topThreeStudents)
            {
                Console.WriteLine($"Student Number: {s.Student.StudentNumber}");
                Console.WriteLine($"Name: {s.Student.FirstName} {s.Student.LastName}");
                Console.WriteLine($"Average Grade: {s.AverageScore:F2}");
                Console.WriteLine("----------------------------------");
            }
        }
    }
}
