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
            // 1. Read JSON file contents into strings.
            string studentsJson = File.ReadAllText("students.json");
            string scoresJson = File.ReadAllText("scores.json");

            // 2. Deserialize the JSON strings into lists of C# objects.
            List<Student>? students = JsonSerializer.Deserialize<List<Student>>(studentsJson);
            List<Score>? scores = JsonSerializer.Deserialize<List<Score>>(scoresJson);
            if (students == null || scores == null)
            {
                Console.WriteLine("Error: Failed to load student or score data. Please check JSON files and their content.");
                return; 
            }

            // 3. Calculate the average score for each student using LINQ.
            var studentGrades = from student in students
                                join score in scores on student.StudentNumber equals score.StudentNumber
                                group score by student into studentGroup
                                select new
                                {
                                    Student = studentGroup.Key,
                                    AverageScore = studentGroup.Average(s => s.Value) 
                                };

            // 4. Order the students by their average score in descending order and select the top 3.
            var topThreeStudents = studentGrades.OrderByDescending(sg => sg.AverageScore) 
                                                .Take(3);                                 


            // 5. Print the information for the top three students to the console.
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