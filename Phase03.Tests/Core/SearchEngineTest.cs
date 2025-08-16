using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using SearchEngine.Core.Interface;
using Xunit;

namespace SearchEngine.Tests
{
    public class SearchEngineTests
    {
        private readonly ISearcher _searcher;
        private readonly IInvertedIndex _invertedIndex;
        private readonly ISearchQuery _searchQuery;
        private readonly SearchEngine _sut;

        public SearchEngineTests()
        {
            _searcher = Substitute.For<ISearcher>();
            _invertedIndex = Substitute.For<IInvertedIndex>();
            _searchQuery = Substitute.For<ISearchQuery>();
            _sut = new SearchEngine();
        }

        [Fact]
        public void Search_WhenCalled_ShouldCallSmartSearchOnSearcherAndReturnResult()
        {
            // Arrange
            var expectedResult = new List<string> { "doc1", "doc2" };
            _searcher.SmartSearch(_searchQuery, _invertedIndex)
                     .Returns(expectedResult);

            // Act
            var result = _sut.Search(_searchQuery, _searcher, _invertedIndex);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _searcher.Received(1).SmartSearch(_searchQuery, _invertedIndex);
        }

        [Fact]
        public void Search_WhenSmartSearchReturnsEmpty_ShouldReturnEmptyEnumerable()
        {
            // Arrange
            var expectedResult = Enumerable.Empty<string>();
            _searcher.SmartSearch(_searchQuery, _invertedIndex)
                     .Returns(expectedResult);

            // Act
            var result = _sut.Search(_searchQuery, _searcher, _invertedIndex);

            // Assert
            result.Should().BeEmpty();
            _searcher.Received(1).SmartSearch(_searchQuery, _invertedIndex);
        }
    }
}