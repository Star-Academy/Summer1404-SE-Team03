using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using SearchEngine.Core.Interface;
using SearchEngine.Core.Model;
using SearchEngine.UI;
using Xunit;

namespace SearchEngine.UnitTests.UI
{
    public class ConsoleUiTests : IDisposable
    {
        private readonly INormalizer _normalizer;
        private readonly ConsoleUi _sut;
        private readonly StringWriter _stringWriter;
        private readonly TextWriter _originalOutput;
        private readonly TextReader _originalInput;

        public ConsoleUiTests()
        {
            _normalizer = Substitute.For<INormalizer>();
            _sut = new ConsoleUi();
            _stringWriter = new StringWriter();
            _originalOutput = Console.Out;
            _originalInput = Console.In;
            Console.SetOut(_stringWriter);

            _normalizer.Normalize(Arg.Any<IEnumerable<string>>())
                .Returns(callInfo => callInfo.Arg<IEnumerable<string>>().Select(t => t.ToUpperInvariant()));
        }

        [Theory]
        [InlineData("must +atLeast -exclude", new[] { "MUST" }, new[] { "ATLEAST" }, new[] { "EXCLUDE" })]
        [InlineData("word1 word2", new[] { "WORD1", "WORD2" }, new string[0], new string[0])]
        [InlineData("+option1 +option2", new string[0], new[] { "OPTION1", "OPTION2" }, new string[0])]
        [InlineData("-ignore1 -ignore2", new string[0], new string[0], new[] { "IGNORE1", "IGNORE2" })]
        [InlineData("  leading   +middle   -trailing  ", new[] { "LEADING" }, new[] { "MIDDLE" }, new[] { "TRAILING" })]
        [InlineData("", new string[0], new string[0], new string[0])]
        [InlineData("   ", new string[0], new string[0], new string[0])]
        public void GetQueryFromUser_WhenGivenUserInput_ShouldParseAndNormalizeCorrectly(
            string input,
            IEnumerable<string> expectedMust,
            IEnumerable<string> expectedAtLeast,
            IEnumerable<string> expectedExclude)
        {
            // Arrange
            SetConsoleInput(input);

            // Act
            var result = _sut.GetQueryFromUser<SearchQuery>(_normalizer);

            // Assert
            result.MustInclude.Should().BeEquivalentTo(expectedMust);
            result.AtLeastOne.Should().BeEquivalentTo(expectedAtLeast);
            result.MustExclude.Should().BeEquivalentTo(expectedExclude);
            _normalizer.Received(3).Normalize(Arg.Any<IEnumerable<string>>());
        }

        [Fact]
        public void GetQueryFromUser_WhenConsoleInputIsEmptyOrNull_ShouldReturnEmptyQuery()
        {
            // Arrange
            SetConsoleInput(string.Empty);

            // Act
            var queryWithEmpty = _sut.GetQueryFromUser<SearchQuery>(_normalizer);
            var queryWithNull = _sut.GetQueryFromUser<SearchQuery>(_normalizer);

            // Assert
            queryWithEmpty.MustInclude.Should().BeEmpty();
            queryWithEmpty.AtLeastOne.Should().BeEmpty();
            queryWithEmpty.MustExclude.Should().BeEmpty();

            queryWithNull.MustInclude.Should().BeEmpty();
            queryWithNull.AtLeastOne.Should().BeEmpty();
            queryWithNull.MustExclude.Should().BeEmpty();
        }

        [Fact]
        public void DisplayResults_WhenResultsExist_ShouldPrintFileNamesToConsole()
        {
            // Arrange
            var results = new List<string>
            {
                @"/docs/file1.txt",
                @"/home/user/document.pdf"
            };
            var expectedOutput = string.Join(
                Environment.NewLine,
                "\nSearch results:",
                "file1.txt",
                "document.pdf",
                ""
            );

            // Act
            _sut.DisplayResults(results);

            // Assert
            _stringWriter.ToString().Should().Be(expectedOutput);
        }

        [Fact]
        public void DisplayResults_WhenNoResultsExist_ShouldPrintNotFoundMessageToConsole()
        {
            // Arrange
            var results = Enumerable.Empty<string>();
            var expectedOutput = string.Join(
                Environment.NewLine,
                "\nSearch results:",
                "No documents found.",
                ""
            );

            // Act
            _sut.DisplayResults(results);

            // Assert
            _stringWriter.ToString().Should().Be(expectedOutput);
        }

        private void SetConsoleInput(string input)
        {
            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);
        }

        public void Dispose()
        {
            Console.SetOut(_originalOutput);
            Console.SetIn(_originalInput);
            _stringWriter.Dispose();
        }
    }
}