using SearchEngine.Core.Processing;
using Xunit;

namespace Phase03.Tests.Core.Processing
{
    public class NormalizerTests
    {
        private readonly Normalizer _normalizer = new();

        [Theory]
        [InlineData("Hello, World!", "HELLO WORLD")]
        [InlineData("Multiple   spaces", "MULTIPLE SPACES")]
        [InlineData("Tabs\tand\nNewLines", "TABS AND NEWLINES")]
        public void Normalize_RemovesPunctuation_ConsolidatesWhitespace_Uppercases(string input, string expected)
        {
            var result = _normalizer.Normalize(input);
            Assert.Equal(expected, result);
        }
    }
}
