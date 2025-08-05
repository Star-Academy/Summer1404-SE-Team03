using System;
using System.Linq;
using SearchEngine.Core;
using SearchEngine.Core.Model;
using SearchEngine.Core.Processing;
using NSubstitute;
using Xunit;

namespace Phase03.Tests.Core
{
    public class SearcherTests
    {
        private InvertedIndexManager BuildIndex()
        {
            var tokenizer = Substitute.For<ITokenizer>();
            var manager = new InvertedIndexManager(tokenizer);

            manager.AddTokenToIndex("A", "doc1", 0);
            manager.AddTokenToIndex("B", "doc1", 1);
            manager.AddTokenToIndex("B", "doc2", 0);
            manager.AddTokenToIndex("C", "doc3", 0);
            
            return manager;
        }

        [Fact]
        public void SmartSearch_WithMustInclude_ReturnsIntersection()
        {
            var manager = BuildIndex();
            var searcher = new Searcher(manager);
            var query = new SearchQuery(new[] { "a", "B" }, Array.Empty<string>(), Array.Empty<string>());
            
            var results = searcher.SmartSearch(query).ToList();
            
            Assert.Single(results);
            Assert.Contains("doc1", results);
        }

        [Fact]
        public void SmartSearch_AtLeastOneAndMustExclude_AppliesFilters()
        {
            var manager = BuildIndex();
            var searcher = new Searcher(manager);
            var query = new SearchQuery(
                Array.Empty<string>(),
                new[] { "b", "c" },
                new[] { "c" });
            
            var results = searcher.SmartSearch(query).ToList();

            Assert.Equal(2, results.Count);
            Assert.Contains("doc1", results);
            Assert.Contains("doc2", results);
        }

        [Fact]
        public void SmartSearch_WithPhraseInMustInclude_ReturnsCorrectDocument()
        {
            var manager = BuildIndex();
            manager.AddTokenToIndex("SEARCH", "doc4", 0);
            manager.AddTokenToIndex("ENGINE", "doc4", 1);
            manager.AddTokenToIndex("SEARCH", "doc5", 0);
            manager.AddTokenToIndex("THE", "doc5", 1);
            manager.AddTokenToIndex("ENGINE", "doc5", 2);

            var searcher = new Searcher(manager);
            var query = new SearchQuery(new[] { "\"search engine\"" }, Array.Empty<string>(), Array.Empty<string>());

            var results = searcher.SmartSearch(query).ToList();

            Assert.Single(results);
            Assert.Contains("doc4", results);
        }
    }
}