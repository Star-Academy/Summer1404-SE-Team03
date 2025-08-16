using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using SearchEngine.Core;
using SearchEngine.Core.Interface;
using Xunit;

namespace SearchEngine.Tests.Core
{
    public class SearcherTests
    {
        private readonly Searcher _sut;
        private readonly IInvertedIndex _invertedIndex;
        private readonly ISearchQuery _query;

        public SearcherTests()
        {
            _sut = new Searcher();
            _invertedIndex = Substitute.For<IInvertedIndex>();
            _query = Substitute.For<ISearchQuery>();

            var allDocs = new List<string> { "doc1", "doc2", "doc3", "doc4" };
            _invertedIndex.GetAllDocuments().Returns(allDocs);
            _invertedIndex.GetDocumentsForToken("MUST1").Returns(new HashSet<string> { "doc1", "doc2" });
            _invertedIndex.GetDocumentsForToken("MUST2").Returns(new HashSet<string> { "doc2", "doc3" });
            _invertedIndex.GetDocumentsForToken("ATLEAST1").Returns(new HashSet<string> { "doc1", "doc4" });
            _invertedIndex.GetDocumentsForToken("ATLEAST2").Returns(new HashSet<string> { "doc3", "doc4" });
            _invertedIndex.GetDocumentsForToken("EXCLUDE1").Returns(new HashSet<string> { "doc1", "doc3" });
            _invertedIndex.GetDocumentsForToken("EXCLUDE2").Returns(new HashSet<string> { "doc4" });
        }

        private void SetupQuery(
            IEnumerable<string> must = null,
            IEnumerable<string> atLeast = null,
            IEnumerable<string> exclude = null)
        {
            _query.MustInclude.Returns(must ?? Enumerable.Empty<string>());
            _query.AtLeastOne.Returns(atLeast ?? Enumerable.Empty<string>());
            _query.MustExclude.Returns(exclude ?? Enumerable.Empty<string>());
        }

        [Fact]
        public void SmartSearch_WithOnlyMustIncludeTokens_ShouldReturnIntersectionOfDocuments()
        {
            // Arrange
            SetupQuery(must: new[] { "MUST1", "MUST2" });

            // Act
            var result = _sut.SmartSearch(_query, _invertedIndex);

            // Assert
            result.Should().ContainSingle().Which.Should().Be("doc2");
        }

        [Fact]
        public void SmartSearch_WithANonExistingMustIncludeToken_ShouldReturnEmpty()
        {
            // Arrange
            SetupQuery(must: new[] { "MUST1", "NOTFOUND" });

            // Act
            var result = _sut.SmartSearch(_query, _invertedIndex);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void SmartSearch_WithOnlyAtLeastOneTokens_ShouldReturnUnionOfDocuments()
        {
            // Arrange
            SetupQuery(atLeast: new[] { "ATLEAST1", "ATLEAST2" });

            // Act
            var result = _sut.SmartSearch(_query, _invertedIndex);

            // Assert
            result.Should().BeEquivalentTo("doc1", "doc3", "doc4");
        }

        [Fact]
        public void SmartSearch_WithOnlyMustExcludeTokens_ShouldReturnAllDocumentsWithoutExcluded()
        {
            // Arrange
            SetupQuery(exclude: new[] { "EXCLUDE1", "EXCLUDE2" });

            // Act
            var result = _sut.SmartSearch(_query, _invertedIndex);

            // Assert
            result.Should().ContainSingle().Which.Should().Be("doc2");
        }

        [Fact]
        public void SmartSearch_WithNoCriteria_ShouldReturnAllDocuments()
        {
            // Arrange
            SetupQuery();

            // Act
            var result = _sut.SmartSearch(_query, _invertedIndex);

            // Assert
            result.Should().BeEquivalentTo("doc1", "doc2", "doc3", "doc4");
        }

        [Fact]
        public void SmartSearch_WithMustIncludeAndAtLeastOneTokensWithNoIntersection_ShouldReturnEmpty()
        {
            // Arrange
            SetupQuery(must: new[] { "MUST1" }, atLeast: new[] { "ATLEAST2" });

            // Act
            var result = _sut.SmartSearch(_query, _invertedIndex);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void SmartSearch_WithMustIncludeAndMustExcludeTokens_ShouldReturnCorrectlyFilteredDocuments()
        {
            // Arrange
            SetupQuery(must: new[] { "MUST1", "MUST2" }, exclude: new[] { "EXCLUDE1" });

            // Act
            var result = _sut.SmartSearch(_query, _invertedIndex);

            // Assert
            result.Should().ContainSingle().Which.Should().Be("doc2");
        }

        [Fact]
        public void SmartSearch_WithAtLeastOneAndMustExcludeTokens_ShouldReturnCorrectlyFilteredDocuments()
        {
            // Arrange
            SetupQuery(atLeast: new[] { "ATLEAST1", "ATLEAST2" }, exclude: new[] { "EXCLUDE1" });

            // Act
            var result = _sut.SmartSearch(_query, _invertedIndex);

            // Assert
            result.Should().ContainSingle().Which.Should().Be("doc4");
        }

        [Fact]
        public void SmartSearch_WithAllCriteria_ShouldReturnCorrectlyFilteredDocuments()
        {
            // Arrange
            SetupQuery(must: new[] { "MUST1" }, atLeast: new[] { "ATLEAST1", "ATLEAST2" }, exclude: new[] { "EXCLUDE2" });

            // Act
            var result = _sut.SmartSearch(_query, _invertedIndex);

            // Assert
            result.Should().ContainSingle().Which.Should().Be("doc1");
        }

        [Fact]
        public void SmartSearch_WhenMustIncludeResultsInEmptySet_ShouldReturnEmptyImmediately()
        {
            // Arrange
            SetupQuery(must: new[] { "MUST1", "NOTFOUND" }, atLeast: new[] { "ATLEAST1" }, exclude: new[] { "EXCLUDE1" });

            // Act
            var result = _sut.SmartSearch(_query, _invertedIndex);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void SmartSearch_WithEmptyMustIncludeList_ShouldUseAllDocsAsBaseAndThenFilter()
        {
            // Arrange
            SetupQuery(must: new string[0], atLeast: new[] { "ATLEAST1" }, exclude: new[] { "EXCLUDE2" });

            // Act
            var result = _sut.SmartSearch(_query, _invertedIndex);

            // Assert
            result.Should().ContainSingle().Which.Should().Be("doc1");
        }
    }
}