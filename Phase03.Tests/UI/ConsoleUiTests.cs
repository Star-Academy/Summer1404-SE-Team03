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
    }
}
