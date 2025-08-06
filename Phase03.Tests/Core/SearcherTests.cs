using Xunit;
using Moq;
using SearchEngine.Core.Interface;
using SearchEngine.Core;
using System.Collections.Generic;
using System.Linq;

namespace SearchEngine.Tests.Core
{
    public class SearcherTests
    {
        private readonly Searcher _searcher;
        private readonly Mock<IInvertedIndex> _mockIndexManager;
        private readonly Mock<ISearchQuery> _mockQuery;

        public SearcherTests()
        {
            _searcher = new Searcher();
            _mockIndexManager = new Mock<IInvertedIndex>();
            _mockQuery = new Mock<ISearchQuery>();

            var allDocs = new List<string> { "doc1", "doc2", "doc3", "doc4" };
            _mockIndexManager.Setup(i => i.GetAllDocuments()).Returns(allDocs);
            _mockIndexManager.Setup(i => i.GetDocumentsForToken("MUST1")).Returns(new List<string> { "doc1", "doc2" });
            _mockIndexManager.Setup(i => i.GetDocumentsForToken("MUST2")).Returns(new List<string> { "doc2", "doc3" });
            _mockIndexManager.Setup(i => i.GetDocumentsForToken("ATLEAST1")).Returns(new List<string> { "doc1", "doc4" });
            _mockIndexManager.Setup(i => i.GetDocumentsForToken("ATLEAST2")).Returns(new List<string> { "doc3", "doc4" });
            _mockIndexManager.Setup(i => i.GetDocumentsForToken("EXCLUDE1")).Returns(new List<string> { "doc1", "doc3" });
            _mockIndexManager.Setup(i => i.GetDocumentsForToken("EXCLUDE2")).Returns(new List<string> { "doc4" });
            _mockIndexManager.Setup(i => i.GetDocumentsForToken(It.Is<string>(s => !new[] { "MUST1", "MUST2", "ATLEAST1", "ATLEAST2", "EXCLUDE1", "EXCLUDE2" }.Contains(s))))
                             .Returns(Enumerable.Empty<string>());
        }

        private void SetupQuery(IEnumerable<string> must, IEnumerable<string> atLeast, IEnumerable<string> exclude)
        {
            _mockQuery.Setup(q => q.MustInclude).Returns(must ?? Enumerable.Empty<string>());
            _mockQuery.Setup(q => q.AtLeastOne).Returns(atLeast ?? Enumerable.Empty<string>());
            _mockQuery.Setup(q => q.MustExclude).Returns(exclude ?? Enumerable.Empty<string>());
        }

        [Fact]
        public void SmartSearch_WithOnlyMustInclude_ReturnsIntersectionOfDocuments()
        {
            SetupQuery(new[] { "must1", "must2" }, null, null);

            var result = _searcher.SmartSearch(_mockQuery.Object, _mockIndexManager.Object);

            Assert.Collection(result, item => Assert.Equal("doc2", item));
        }

        [Fact]
        public void SmartSearch_WithMustIncludeTermNotFound_ReturnsEmpty()
        {
            SetupQuery(new[] { "must1", "notfound" }, null, null);

            var result = _searcher.SmartSearch(_mockQuery.Object, _mockIndexManager.Object);

            Assert.Empty(result);
        }

        [Fact]
        public void SmartSearch_WithOnlyAtLeastOne_ReturnsUnionOfDocuments()
        {
            SetupQuery(null, new[] { "atleast1", "atleast2" }, null);

            var result = _searcher.SmartSearch(_mockQuery.Object, _mockIndexManager.Object).ToList();

            Assert.Equal(3, result.Count);
            Assert.Contains("doc1", result);
            Assert.Contains("doc3", result);
            Assert.Contains("doc4", result);
        }

        [Fact]
        public void SmartSearch_WithOnlyMustExclude_ReturnsDocumentsWithoutExcluded()
        {
            SetupQuery(null, null, new[] { "exclude1", "exclude2" });

            var result = _searcher.SmartSearch(_mockQuery.Object, _mockIndexManager.Object);

            Assert.Collection(result, item => Assert.Equal("doc2", item));
        }

        [Fact]
        public void SmartSearch_WithNoCriteria_ReturnsAllDocuments()
        {
            SetupQuery(null, null, null);

            var result = _searcher.SmartSearch(_mockQuery.Object, _mockIndexManager.Object);

            Assert.Equal(_mockIndexManager.Object.GetAllDocuments(), result);
        }

        [Fact]
        public void SmartSearch_WithMustIncludeAndAtLeastOne_ReturnsCorrectIntersection()
        {
            SetupQuery(new[] { "must1" }, new[] { "atleast2" }, null);

            var result = _searcher.SmartSearch(_mockQuery.Object, _mockIndexManager.Object).ToList();

            Assert.Empty(result);
        }

        [Fact]
        public void SmartSearch_WithMustIncludeAndMustExclude_ReturnsCorrectDifference()
        {
            SetupQuery(new[] { "must1", "must2" }, null, new[] { "exclude1" });

            var result = _searcher.SmartSearch(_mockQuery.Object, _mockIndexManager.Object);

            Assert.Collection(result, item => Assert.Equal("doc2", item));
        }
        
        [Fact]
        public void SmartSearch_WithAtLeastOneAndMustExclude_ReturnsCorrectResult()
        {
            SetupQuery(null, new[] { "atleast1", "atleast2" }, new[] { "exclude1" });
            
            var result = _searcher.SmartSearch(_mockQuery.Object, _mockIndexManager.Object);
            
            Assert.Collection(result.OrderBy(d => d), 
                item => Assert.Equal("doc4", item)
            );
        }

        [Fact]
        public void SmartSearch_WithAllCriteria_ReturnsCorrectResult()
        {
            SetupQuery(new[] { "must1" }, new[] { "atleast1", "atleast2" }, new[] { "exclude2" });

            var result = _searcher.SmartSearch(_mockQuery.Object, _mockIndexManager.Object);

            Assert.Collection(result, item => Assert.Equal("doc1", item));
        }
        
        [Fact]
        public void SmartSearch_MustIncludeIsEmpty_IgnoresFurtherFiltersAndReturnsEmpty()
        {
            SetupQuery(new[] { "must1", "notfound" }, new[] { "atleast1" }, new[] { "exclude1" });

            var result = _searcher.SmartSearch(_mockQuery.Object, _mockIndexManager.Object);

            Assert.Empty(result);
        }

        [Fact]
        public void SmartSearch_WithEmptyInitialMustInclude_FallsBackToAllDocsAndFilters()
        {
            SetupQuery(new string[0], new[] { "atleast1" }, new[] { "exclude2" });

            var result = _searcher.SmartSearch(_mockQuery.Object, _mockIndexManager.Object);
            
            Assert.Collection(result, item => Assert.Equal("doc1", item));
        }
    }
}