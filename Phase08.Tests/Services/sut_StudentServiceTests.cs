using FluentAssertions;
using NSubstitute;
using ScoreManager.Interfaces;
using ScoreManager.Models;
using ScoreManager.Services;

namespace Phase08.Tests.Services
{
    public class sut_StudentServiceTests
    {
        private readonly IDbExecuter _dbExecuterMock;
        private readonly StudentService _sut;

        public sut_StudentServiceTests()
        {
            _dbExecuterMock = Substitute.For<IDbExecuter>();
            _sut = new StudentService(_dbExecuterMock);
        }

        [Fact]
        public async Task AddNewStudentAsync_ShouldThrowArgumentException_WhenFirstNameIsWhitespace()
        {
            var studentNumber = 1;
            var firstName = " ";
            var lastName = "LastName";

            Func<Task> act = async () => await _sut.AddNewStudentAsync(studentNumber, firstName, lastName);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("First and last names cannot be empty.");
        }

        [Fact]
        public async Task AddNewStudentAsync_ShouldThrowArgumentException_WhenLastNameIsEmpty()
        {
            var studentNumber = 1;
            var firstName = "FirstName";
            var lastName = "";

            Func<Task> act = async () => await _sut.AddNewStudentAsync(studentNumber, firstName, lastName);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("First and last names cannot be empty.");
        }

        [Fact]
        public async Task AddNewStudentAsync_ShouldCallDbExecuter_WhenInputIsValid()
        {
            var studentNumber = 1;
            var firstName = "FirstName";
            var lastName = "LastName";
            var expectedStudent = new Student { StudentNumber = studentNumber, FirstName = firstName, LastName = lastName };

            _dbExecuterMock.AddStudentAsync(Arg.Any<IStudent>()).Returns(expectedStudent);

            var result = await _sut.AddNewStudentAsync(studentNumber, firstName, lastName);

            result.Should().BeEquivalentTo(expectedStudent);
            await _dbExecuterMock.Received(1).AddStudentAsync(Arg.Is<IStudent>(s =>
                s.StudentNumber == studentNumber &&
                s.FirstName == firstName &&
                s.LastName == lastName));
        }

        [Fact]
        public async Task FindStudentByNumberAsync_ShouldReturnStudentFromDbExecuter()
        {
            var studentNumber = 1;
            var expectedStudent = new Student { StudentNumber = studentNumber, FirstName = "Test", LastName = "User" };
            _dbExecuterMock.GetStudentByNumberAsync(studentNumber).Returns(expectedStudent);

            var result = await _sut.FindStudentByNumberAsync(studentNumber);

            result.Should().Be(expectedStudent);
            await _dbExecuterMock.Received(1).GetStudentByNumberAsync(studentNumber);
        }

        [Fact]
        public async Task GetAllStudentsAsync_ShouldReturnAllStudentsFromDbExecuter()
        {
            var expectedStudents = new List<IStudent> { new Student(), new Student() };
            _dbExecuterMock.GetAllStudentsWithScoresAsync().Returns(expectedStudents);

            var result = await _sut.GetAllStudentsAsync();

            result.Should().BeEquivalentTo(expectedStudents);
            await _dbExecuterMock.Received(1).GetAllStudentsWithScoresAsync();
        }
    }
}