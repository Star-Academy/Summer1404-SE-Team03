using Phase01;
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
            var studentsJson = File.ReadAllText("students.json");
            var scoresJson = File.ReadAllText("scores.json");

            List<Student>? students = JsonSerializer.Deserialize<List<Student>>(studentsJson);
            List<Score>? scores = JsonSerializer.Deserialize<List<Score>>(scoresJson);
            if (students == null || scores == null)
            {
                Console.WriteLine("Error: Failed to load student or score data. Please check JSON files and their content.");
                return;
            }

            var studentGrades = students.Join(scores,
                    student => student.StudentNumber,
                    score => score.StudentNumber,
                    (student, score) => new { student, score }
                )
                .GroupBy(
                    joinedData => joinedData.student,
                    joinedData => joinedData.score.Value
                )
                .Select(studentGroup => new
                {
                    Student = studentGroup.Key,
                    AverageScore = studentGroup.Average()
                });

            var topThreeStudents = studentGrades.OrderByDescending(sg => sg.AverageScore)
                                                .Take(3);


            Console.WriteLine("Top Three Students (by Average Score):");
            Console.WriteLine("----------------------------------");
            foreach (var studentInfo in topThreeStudents)
            {
                Console.WriteLine($"Student Number: {studentInfo.Student?.StudentNumber}");
                Console.WriteLine($"Name: {studentInfo.Student?.FirstName} {studentInfo.Student?.LastName}");

                Console.WriteLine($"Average Grade: {studentInfo.AverageScore:F2}");
                Console.WriteLine("----------------------------------");
            }

            Console.WriteLine("\nOperation completed successfully. Press any key to exit.");
            Console.ReadKey();
        }
    }
}