using Xunit;
using SearchEngine.Core.Model;
using System.Collections.Generic;
using System.Linq;

namespace SearchEngine.Tests.Core.Model
{
    public class SearchQueryTests
    {
        [Fact]
        public void DefaultConstructor_ShouldInitializePropertiesAsEmptyEnumerables()
        {
            var searchQuery = new SearchQuery();

            Assert.NotNull(searchQuery.MustInclude);
            Assert.Empty(searchQuery.MustInclude);
            Assert.NotNull(searchQuery.AtLeastOne);
            Assert.Empty(searchQuery.AtLeastOne);
            Assert.NotNull(searchQuery.MustExclude);
            Assert.Empty(searchQuery.MustExclude);
        }

        [Fact]
        public void ParameterizedConstructor_ShouldAssignPropertiesCorrectly()
        {
            var mustInclude = new List<string> { "term1", "term2" };
            var atLeastOne = new List<string> { "term3", "term4" };
            var mustExclude = new List<string> { "term5" };

            var searchQuery = new SearchQuery(mustInclude, atLeastOne, mustExclude);

            Assert.Equal(mustInclude, searchQuery.MustInclude);
            Assert.Equal(atLeastOne, searchQuery.AtLeastOne);
            Assert.Equal(mustExclude, searchQuery.MustExclude);
        }

        [Fact]
        public void Properties_ShouldBeSettable()
        {
            var searchQuery = new SearchQuery();
            var mustInclude = new List<string> { "new_term1" };
            var atLeastOne = new List<string> { "new_term2" };
            var mustExclude = new List<string> { "new_term3" };

            searchQuery.MustInclude = mustInclude;
            searchQuery.AtLeastOne = atLeastOne;
            searchQuery.MustExclude = mustExclude;

            Assert.Equal(mustInclude, searchQuery.MustInclude);
            Assert.Equal(atLeastOne, searchQuery.AtLeastOne);
            Assert.Equal(mustExclude, searchQuery.MustExclude);
        }
    }
}