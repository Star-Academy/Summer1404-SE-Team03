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
            manager.AddTokenToIndex("A", "doc1");
            manager.AddTokenToIndex("B", "doc1");
            manager.AddTokenToIndex("B", "doc2");
            manager.AddTokenToIndex("C", "doc3");
            return manager;
        }

        [Fact]
        public void SmartSearch_WithMustInclude_ReturnsIntersection()
        {
            var manager = BuildIndex();
            var searcher = new Searcher(manager);
            var query = new SearchQuery(new[] { "A", "B" }, Array.Empty<string>(), Array.Empty<string>());
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
                new[] { "B", "C" },
                new[] { "C" });
            var results = searcher.SmartSearch(query).ToList();
            Assert.Equal(2, results.Count);
            Assert.Contains("doc1", results);
            Assert.Contains("doc2", results);
        }
    }
}
