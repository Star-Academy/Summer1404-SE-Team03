using System;
using System.Collections.Generic;
using FluentAssertions;
using SearchEngine.Core.Processing;
using Xunit;

namespace SearchEngine.Tests.Core.Processing
{
    public class TokenizerTests
    {
        private readonly Tokenizer _sut;

        public TokenizerTests()
        {
            _sut = new Tokenizer();
        }

        [Theory]
        [InlineData("hello world", new[] { "hello", "world" })]
        [InlineData("this  is   a    test", new[] { "this", "is", "a", "test" })]
        [InlineData("  leading and trailing spaces  ", new[] { "leading", "and", "trailing", "spaces" })]
        [InlineData("singleword", new[] { "singleword" })]
        [InlineData("another-test with-hyphens", new[] { "another-test", "with-hyphens" })]
        public void Tokenize_WhenGivenAString_ShouldReturnCorrectTokens(string input, IEnumerable<string> expected)
        {
            // Act
            var result = _sut.Tokenize(input);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Tokenize_WhenGivenAnEmptyString_ShouldReturnEmptyEnumerable()
        {
            // Arrange
            var input = string.Empty;

            // Act
            var result = _sut.Tokenize(input);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Tokenize_WhenGivenAWhitespaceString_ShouldReturnEmptyEnumerable()
        {
            // Arrange
            var input = "      ";

            // Act
            var result = _sut.Tokenize(input);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Tokenize_WhenGivenNullInput_ShouldThrowNullReferenceException()
        {
            // Arrange
            Action act = () => _sut.Tokenize(null);

            // Act & Assert
            act.Should().Throw<NullReferenceException>();
        }
    }
}