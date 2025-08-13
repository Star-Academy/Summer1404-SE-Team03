using FluentAssertions;
using NSubstitute;
using ScoreManager.DTOs;
using ScoreManager.Interfaces;
using ScoreManager.Models;
using ScoreManager.Services;

namespace Phase08.Tests.Services
{
    public class sut_ReportingServiceTests
    {
        private readonly IDbExecuter _dbExecuterMock;
        private readonly ReportingService _sut;

        public sut_ReportingServiceTests()
        {
            _dbExecuterMock = Substitute.For<IDbExecuter>();
            _sut = new ReportingService(_dbExecuterMock);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetTopStudentsByAverageScoreAsync_ShouldReturnEmptyList_WhenCountIsZeroOrLess(int count)
        {
            var result = await _sut.GetTopStudentsByAverageScoreAsync(count);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetTopStudentsByAverageScoreAsync_ShouldReturnEmptyList_WhenNoStudentsInDatabase()
        {
            _dbExecuterMock.GetAllStudentsWithScoresAsync().Returns(new List<IStudent>());

            var result = await _sut.GetTopStudentsByAverageScoreAsync(10);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetTopStudentsByAverageScoreAsync_ShouldCorrectlyCalculateAverageAndRankStudents()
        {
            var students = new List<IStudent>
            {
                new Student { StudentNumber = 1, FirstName = "Low", LastName = "Scorer", Scores = new List<Score> { new Score { Value = 10 }, new Score { Value = 12 } } }, // Avg 11
                new Student { StudentNumber = 2, FirstName = "High", LastName = "Scorer", Scores = new List<Score> { new Score { Value = 20 }, new Score { Value = 18 } } }, // Avg 19
                new Student { StudentNumber = 3, FirstName = "Mid", LastName = "Scorer", Scores = new List<Score> { new Score { Value = 15 }, new Score { Value = 15 } } }, // Avg 15
                new Student { StudentNumber = 4, FirstName = "No", LastName = "Scores", Scores = new List<Score>() } // Avg 0
            };
            _dbExecuterMock.GetAllStudentsWithScoresAsync().Returns(students);
            var expectedOrder = new[] { 2, 3, 1, 4 };

            var result = await _sut.GetTopStudentsByAverageScoreAsync(4);

            result.Should().HaveCount(4);
            result.Select(s => s.StudentNumber).Should().ContainInOrder(expectedOrder);
            result.First(s => s.StudentNumber == 2).AverageScore.Should().Be(19);
            result.First(s => s.StudentNumber == 3).AverageScore.Should().Be(15);
            result.First(s => s.StudentNumber == 1).AverageScore.Should().Be(11);
            result.First(s => s.StudentNumber == 4).AverageScore.Should().Be(0);
        }

        [Fact]
        public async Task GetTopStudentsByAverageScoreAsync_ShouldReturnOnlyTopNStudents()
        {
            var students = new List<IStudent>
            {
                new Student { StudentNumber = 1, FirstName = "A", LastName = "A", Scores = new List<Score> { new Score { Value = 10 } } },
                new Student { StudentNumber = 2, FirstName = "B", LastName = "B", Scores = new List<Score> { new Score { Value = 20 } } },
                new Student { StudentNumber = 3, FirstName = "C", LastName = "C", Scores = new List<Score> { new Score { Value = 15 } } }
            };
            _dbExecuterMock.GetAllStudentsWithScoresAsync().Returns(students);

            var result = await _sut.GetTopStudentsByAverageScoreAsync(2);

            result.Should().HaveCount(2);
            result.Select(s => s.StudentNumber).Should().ContainInOrder(2, 3);
            result.Select(s => s.StudentNumber).Should().NotContain(1);
        }
    }
}