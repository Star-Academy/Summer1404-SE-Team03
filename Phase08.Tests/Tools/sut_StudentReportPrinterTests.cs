using FluentAssertions;
using ScoreManager.DTOs;
using ScoreManager.Tools;

namespace Phase08.Tests.Tools
{
    public class sut_StudentReportPrinterTests : IDisposable
    {
        private readonly StringWriter _stringWriter;
        private readonly TextWriter _originalOutput;
        private readonly StudentReportPrinter _sut;

        public sut_StudentReportPrinterTests()
        {
            _originalOutput = Console.Out;
            _stringWriter = new StringWriter();
            Console.SetOut(_stringWriter);
            _sut = new StudentReportPrinter();
        }

        public void Dispose()
        {
            Console.SetOut(_originalOutput);
            _stringWriter.Dispose();
        }

        [Fact]
        public void PrintTopStudents_ShouldPrintEmptyMessage_WhenStudentListIsEmpty()
        {
            var students = new List<StudentAverageScoreDto>();

            _sut.PrintTopStudents(students);
            var output = _stringWriter.ToString();

            output.Should().Contain("No student data found to generate a report.");
        }

        [Fact]
        public void PrintTopStudents_ShouldPrintCorrectlyFormattedReport_WhenStudentsExist()
        {
            var students = new List<StudentAverageScoreDto>
            {
                new StudentAverageScoreDto { StudentNumber = 101, FirstName = "John", LastName = "Doe", AverageScore = 18.756 },
                new StudentAverageScoreDto { StudentNumber = 22, FirstName = "Jane", LastName = "Smith", AverageScore = 15.5 }
            };

            _sut.PrintTopStudents(students);
            var output = _stringWriter.ToString();

            output.Should().Contain("---- Top 10 Students by Average Score ----");
            output.Should().Contain("==========================================");
            output.Should().Contain("Rank #1");
            output.Should().Contain("Student ID: 101");
            output.Should().Contain("Name: John Doe");
            output.Should().Contain("Average Score: 18.76");
            output.Should().Contain("Rank #2");
            output.Should().Contain("Student ID: 22");
            output.Should().Contain("Name: Jane Smith");
            output.Should().Contain("Average Score: 15.50");
        }
    }
}