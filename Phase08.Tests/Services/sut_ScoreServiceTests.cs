using FluentAssertions;
using NSubstitute;
using ScoreManager.Interfaces;
using ScoreManager.Models;
using ScoreManager.Services;

namespace Phase08.Tests.Services
{
    public class sut_ScoreServiceTests
    {
        private readonly IDbExecuter _dbExecuterMock;
        private readonly ScoreService _sut;

        public sut_ScoreServiceTests()
        {
            _dbExecuterMock = Substitute.For<IDbExecuter>();
            _sut = new ScoreService(_dbExecuterMock);
        }

        [Fact]
        public async Task AddScoreForStudentAsync_ShouldThrowKeyNotFoundException_WhenStudentNotFound()
        {
            var studentNumber = 999;
            _dbExecuterMock.GetStudentByNumberAsync(studentNumber).Returns((IStudent?)null);

            Func<Task> act = async () => await _sut.AddScoreForStudentAsync(studentNumber, "Math", 15);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Student with number {studentNumber} not found.");
        }

        [Theory]
        [InlineData(-0.1)]
        [InlineData(20.1)]
        public async Task AddScoreForStudentAsync_ShouldThrowArgumentOutOfRangeException_WhenScoreIsInvalid(double invalidScore)
        {
            var studentNumber = 1;
            var student = new Student { StudentNumber = studentNumber };
            _dbExecuterMock.GetStudentByNumberAsync(studentNumber).Returns(student);

            Func<Task> act = async () => await _sut.AddScoreForStudentAsync(studentNumber, "Math", invalidScore);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .Where(e => e.ParamName == "scoreValue");
        }

        [Fact]
        public async Task AddScoreForStudentAsync_ShouldAddScore_WhenInputIsValid()
        {
            var studentNumber = 1;
            var lesson = "Science";
            var scoreValue = 18.5;
            var student = new Student { StudentNumber = studentNumber };
            var expectedScore = new Score { StudentNumber = studentNumber, Lesson = lesson, Value = scoreValue };

            _dbExecuterMock.GetStudentByNumberAsync(studentNumber).Returns(student);
            _dbExecuterMock.AddScoreAsync(Arg.Any<IScore>()).Returns(expectedScore);

            var result = await _sut.AddScoreForStudentAsync(studentNumber, lesson, scoreValue);

            result.Should().BeEquivalentTo(expectedScore);
            await _dbExecuterMock.Received(1).AddScoreAsync(Arg.Is<IScore>(s =>
                s.StudentNumber == studentNumber &&
                s.Lesson == lesson &&
                s.Value == scoreValue));
        }
    }
}