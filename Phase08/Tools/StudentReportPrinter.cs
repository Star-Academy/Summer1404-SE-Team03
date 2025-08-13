using ScoreManager.DTOs;

namespace ScoreManager.Tools
{
    /// <summary>
    /// Handles the presentation of student reports. Its only job is to print. (SRP)
    /// </summary>
    public class StudentReportPrinter
    {
        public void PrintTopStudents(IEnumerable<StudentAverageScoreDto> topStudents)
        {
            Console.WriteLine("---- Top 10 Students by Average Score ----");
            Console.WriteLine("==========================================");

            if (!topStudents.Any())
            {
                Console.WriteLine("No student data found to generate a report.");
                return;
            }

            int rank = 1;
            foreach (var student in topStudents)
            {
                Console.WriteLine(
                    $"Rank #{rank++, -3} | " +
                    $"Student ID: {student.StudentNumber, -5} | " +
                    $"Name: {student.FirstName} {student.LastName, -20} | " +
                    $"Average Score: {student.AverageScore:F2}" // F2 formats the double to 2 decimal places
                );
            }
             Console.WriteLine("==========================================");
        }
    }
}