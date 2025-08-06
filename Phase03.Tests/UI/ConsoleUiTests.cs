using Xunit;
using Moq;
using SearchEngine.Core.Interface;
using SearchEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using SearchEngine.Core.Model;

namespace SearchEngine.UnitTests.UI
{
    public class ConsoleUiTests : IDisposable
    {
        private readonly Mock<INormalizer> _mockNormalizer;
        private readonly ConsoleUi _consoleUi;
        private readonly StringWriter _stringWriter;
        private readonly TextWriter _originalOutput;
        private readonly TextReader _originalInput;

        public ConsoleUiTests()
        {
            _mockNormalizer = new Mock<INormalizer>();
            _consoleUi = new ConsoleUi();
            _originalOutput = Console.Out;
            _stringWriter = new StringWriter();
            Console.SetOut(_stringWriter);
            _originalInput = Console.In;
            _mockNormalizer.Setup(n => n.Normalize(It.IsAny<IEnumerable<string>>()))
                           .Returns<IEnumerable<string>>(tokens => tokens.Select(t => t.ToUpperInvariant()));
        }

        private void SetConsoleInput(string input)
        {
            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);
        }

        [Theory]
        [InlineData("must +atLeast -exclude", new[] { "MUST" }, new[] { "ATLEAST" }, new[] { "EXCLUDE" })]
        [InlineData("word1 word2", new[] { "WORD1", "WORD2" }, new string[0], new string[0])]
        [InlineData("+option1 +option2", new string[0], new[] { "OPTION1", "OPTION2" }, new string[0])]
        [InlineData("-ignore1 -ignore2", new string[0], new string[0], new[] { "IGNORE1", "IGNORE2" })]
        [InlineData("  leading   +middle   -trailing  ", new[] { "LEADING" }, new[] { "MIDDLE" }, new[] { "TRAILING" })]
        [InlineData("", new string[0], new string[0], new string[0])]
        [InlineData("   ", new string[0], new string[0], new string[0])]
        public void GetQueryFromUser_ShouldParseAndNormalizeInputCorrectly(
            string input,
            IEnumerable<string> expectedMust,
            IEnumerable<string> expectedAtLeast,
            IEnumerable<string> expectedExclude)
        {
            SetConsoleInput(input);
            var result = _consoleUi.GetQueryFromUser<SearchQuery>(_mockNormalizer.Object);
            Assert.Equal(expectedMust, result.MustInclude);
            Assert.Equal(expectedAtLeast, result.AtLeastOne);
            Assert.Equal(expectedExclude, result.MustExclude);
            _mockNormalizer.Verify(n => n.Normalize(It.IsAny<IEnumerable<string>>()), Times.Exactly(3));
        }

        [Fact]
        public void GetQueryFromUser_WithNullConsoleInput_ShouldReturnEmptyQuery()
        {
            Console.SetIn(new StringReader(""));
            var queryWithEmpty = _consoleUi.GetQueryFromUser<SearchQuery>(_mockNormalizer.Object);
            var queryWithNull = _consoleUi.GetQueryFromUser<SearchQuery>(_mockNormalizer.Object);

            Assert.Empty(queryWithEmpty.MustInclude);
            Assert.Empty(queryWithEmpty.AtLeastOne);
            Assert.Empty(queryWithEmpty.MustExclude);

            Assert.Empty(queryWithNull.MustInclude);
            Assert.Empty(queryWithNull.AtLeastOne);
            Assert.Empty(queryWithNull.MustExclude);
        }

        [Fact]
        public void DisplayResults_WhenResultsExist_ShouldPrintFileNames()
        {
            var results = new List<string>
            {
                @"/docs/file1.txt",
                @"/home/user/document.pdf"
            };
            var expectedOutputWriter = new StringWriter();
            expectedOutputWriter.WriteLine("\nSearch results:");
            expectedOutputWriter.WriteLine("file1.txt");
            expectedOutputWriter.WriteLine("document.pdf");

            _consoleUi.DisplayResults(results);

            Assert.Equal(expectedOutputWriter.ToString(), _stringWriter.ToString());
        }

        [Fact]
        public void DisplayResults_WhenNoResults_ShouldPrintNotFoundMessage()
        {
            var results = Enumerable.Empty<string>();
            var expectedOutputWriter = new StringWriter();
            expectedOutputWriter.WriteLine("\nSearch results:");
            expectedOutputWriter.WriteLine("No documents found.");

            _consoleUi.DisplayResults(results);

            Assert.Equal(expectedOutputWriter.ToString(), _stringWriter.ToString());
        }

        public void Dispose()
        {
            Console.SetOut(_originalOutput);
            Console.SetIn(_originalInput);
            _stringWriter.Dispose();
        }
    }
}