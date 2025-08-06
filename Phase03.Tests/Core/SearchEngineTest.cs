using Xunit;
using Moq;
using SearchEngine.Core.Interface;
using System.Collections.Generic;
using System.Linq;

namespace SearchEngine.Tests
{
    public class SearchEngineTests
    {
        private readonly SearchEngine _searchEngine;
        private readonly Mock<ISearcher> _mockSearcher;
        private readonly Mock<IInvertedIndex> _mockInvertedIndex;
        private readonly Mock<ISearchQuery> _mockSearchQuery;

        public SearchEngineTests()
        {
            _searchEngine = new SearchEngine();
            _mockSearcher = new Mock<ISearcher>();
            _mockInvertedIndex = new Mock<IInvertedIndex>();
            _mockSearchQuery = new Mock<ISearchQuery>();
        }

        [Fact]
        public void Search_ShouldCallSmartSearchOnSearcher_AndReturnResult()
        {
            var expectedResult = new List<string> { "doc1", "doc2" };
            _mockSearcher.Setup(s => s.SmartSearch(_mockSearchQuery.Object, _mockInvertedIndex.Object))
                         .Returns(expectedResult);
            
            var result = _searchEngine.Search(_mockSearchQuery.Object, _mockSearcher.Object, _mockInvertedIndex.Object);
            
            Assert.Equal(expectedResult, result);
            
            _mockSearcher.Verify(s => s.SmartSearch(_mockSearchQuery.Object, _mockInvertedIndex.Object), Times.Once());
        }

        [Fact]
        public void Search_WhenSmartSearchReturnsEmpty_ShouldReturnEmpty()
        {
            var expectedResult = Enumerable.Empty<string>();
            _mockSearcher.Setup(s => s.SmartSearch(It.IsAny<ISearchQuery>(), It.IsAny<IInvertedIndex>()))
                         .Returns(expectedResult);
            
            var result = _searchEngine.Search(_mockSearchQuery.Object, _mockSearcher.Object, _mockInvertedIndex.Object);
            
            Assert.Empty(result);
            _mockSearcher.Verify(s => s.SmartSearch(_mockSearchQuery.Object, _mockInvertedIndex.Object), Times.Once());
        }
    }
}