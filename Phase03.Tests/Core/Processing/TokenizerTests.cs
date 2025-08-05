using System.Linq;
using SearchEngine.Core.Processing;
using NSubstitute;
using Xunit;

namespace Phase03.Tests.Core.Processing
{
    public class TokenizerTests
    {
        [Fact]
        public void Tokenize_CallsNormalizeAndSplitsText()
        {
            var normalizer = Substitute.For<INormalizer>();
            normalizer.Normalize("ignored input").Returns("A B  C");
            var tokenizer = new Tokenizer(normalizer);

            var tokens = tokenizer.Tokenize("ignored input").ToList();

            Assert.Equal(new[] { "A", "B", "C" }, tokens);
            normalizer.Received(1).Normalize("ignored input");
        }
    }
}
