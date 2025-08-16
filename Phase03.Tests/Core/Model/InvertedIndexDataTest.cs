using FluentAssertions;
using SearchEngine.Core.Model;
using Xunit;

namespace SearchEngine.Tests.Core.Model
{
    public class InvertedIndexDataTests
    {
        private readonly InvertedIndexData _sut;

        public InvertedIndexDataTests()
        {
            _sut = new InvertedIndexData();
        }

        [Fact]
        public void Constructor_WhenCalled_ShouldInitializeAnEmptyIndex()
        {
            // Act
            var index = _sut.Index;

            // Assert
            index.Should().NotBeNull();
            index.Should().BeEmpty();
        }
    }
}