using Xunit;
using System.Collections.Generic;
using System.Linq;
using SearchEngine.Core.Model;

namespace SearchEngine.Core.Model.Tests
{
    public class SearchQueryTests
    {
        public static IEnumerable<object[]> GetSearchQueryTestData()
        {
            yield return new object[]
            {
                new List<string> { "must1", "must2" },
                new List<string> { "atLeast1", "atLeast2" },
                new List<string> { "exclude1", "exclude2" }
            };

            yield return new object[]
            {
                new List<string>(),
                Enumerable.Empty<string>(),
                new string[] { }
            };

            yield return new object[]
            {
                null,
                null,
                null
            };

            yield return new object[]
            {
                new List<string> { "term" },
                new List<string>(),
                null
            };

            yield return new object[]
            {
                null,
                new List<string> { "optional" },
                new List<string>()
            };
        }

        [Theory]
        [MemberData(nameof(GetSearchQueryTestData))]
        public void Constructor_ShouldCorrectlyAssignProperties(
            IEnumerable<string> mustInclude,
            IEnumerable<string> atLeastOne,
            IEnumerable<string> mustExclude)
        {
            var searchQuery = new SearchQuery(mustInclude, atLeastOne, mustExclude);

            Assert.Same(mustInclude, searchQuery.MustInclude);
            Assert.Same(atLeastOne, searchQuery.AtLeastOne);
            Assert.Same(mustExclude, searchQuery.MustExclude);
        }
    }
}