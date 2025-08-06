using Xunit;
using SearchEngine.Core.Processing;
using System;
using System.Linq;

namespace SearchEngine.Tests.Core.Processing
{
    public class TokenizerTests
    {
        private readonly Tokenizer _tokenizer;

        public TokenizerTests()
        {
            _tokenizer = new Tokenizer();
        }

        [Theory]
        [InlineData("hello world", new[] { "hello", "world" })]
        [InlineData("this  is   a    test", new[] { "this", "is", "a", "test" })]
        [InlineData("  leading and trailing spaces  ", new[] { "leading", "and", "trailing", "spaces" })]
        [InlineData("singleword", new[] { "singleword" })]
        [InlineData("another-test with-hyphens", new[] { "another-test", "with-hyphens" })]
        public void Tokenize_ShouldReturnCorrectTokens(string input, string[] expected)
        {
            var result = _tokenizer.Tokenize(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Tokenize_EmptyString_ShouldReturnEmptyEnumerable()
        {
            var result = _tokenizer.Tokenize(string.Empty);
            Assert.Empty(result);
        }

        [Fact]
        public void Tokenize_WhitespaceString_ShouldReturnEmptyEnumerable()
        {
            var result = _tokenizer.Tokenize("      ");
            Assert.Empty(result);
        }

        [Fact]
        public void Tokenize_NullInput_ShouldThrowException()
        {
            Assert.Throws<NullReferenceException>(() => _tokenizer.Tokenize(null));
        }
    }
}