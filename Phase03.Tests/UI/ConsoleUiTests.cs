using System;
using System.IO;
using System.Linq;
using SearchEngine.Core.Model;
using SearchEngine.UI;
using Xunit;

namespace Phase03.Tests.UI
{
    public class ConsoleUiTests
    {
        [Fact]
        public void GetQueryFromUser_ParsesPlusMinusAndPlainTokens()
        {
            var input = "+one -two three";
            using var reader = new StringReader(input);
            Console.SetIn(reader);
            var consoleUi = new ConsoleUi();

            var query = consoleUi.GetQueryFromUser();

            Assert.Equal(new[] { "three" }, query.MustInclude);
            Assert.Equal(new[] { "one" }, query.AtLeastOne);
            Assert.Equal(new[] { "two" }, query.MustExclude);
        }

        [Fact]
        public void DisplayResults_CheckTheResultPrinted()
        {
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var consoleUi = new ConsoleUi();
            IEnumerable<string> results = new List<string> { "doc1", "doc2" };

            consoleUi.DisplayResults(results);

            var output = consoleOutput.ToString();
            Assert.Contains("doc1", output);
            Assert.Contains("doc2", output);
        }

        [Fact]
        public void DisplayResults_WithNoResult()
        {
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var consoleUi = new ConsoleUi();
            IEnumerable<string> results = new List<string>();

            consoleUi.DisplayResults(results);

            var output = consoleOutput.ToString();
            Assert.Contains("No documents found.", output);
        }
    }
}
