using FluentAssertions;
using SearchEngine.Core.Processing;
using Xunit;

namespace Phase03.Tests.Core.Processing
{
    public class NormalizerTests
    {
        private readonly Normalizer _sut;

        public NormalizerTests()
        {
            _sut = new Normalizer();
        }

        [Theory]
        [InlineData("Hello, World!", "HELLO WORLD")]
        [InlineData("Multiple   spaces", "MULTIPLE SPACES")]
        [InlineData("Tabs\tand\nNewLines", "TABS AND NEWLINES")]
        public void Normalize_ShouldReturnNormalizedString(string input, string expected)
        {
            var result = _sut.Normalize(input);

            result.Should().Be(expected);
        }
    }
}