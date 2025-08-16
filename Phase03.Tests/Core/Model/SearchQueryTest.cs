using System.Collections.Generic;
using FluentAssertions;
using SearchEngine.Core.Model;
using Xunit;

namespace SearchEngine.Tests.Core.Model
{
    public class SearchQueryTests
    {
        [Fact]
        public void DefaultConstructor_WhenCalled_ShouldInitializePropertiesAsEmptyEnumerables()
        {
            // Arrange & Act
            var sut = new SearchQuery();

            // Assert
            sut.MustInclude.Should().NotBeNull().And.BeEmpty();
            sut.AtLeastOne.Should().NotBeNull().And.BeEmpty();
            sut.MustExclude.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void ParameterizedConstructor_WhenCalled_ShouldAssignPropertiesCorrectly()
        {
            // Arrange
            var mustInclude = new List<string> { "term1", "term2" };
            var atLeastOne = new List<string> { "term3", "term4" };
            var mustExclude = new List<string> { "term5" };

            // Act
            var sut = new SearchQuery(mustInclude, atLeastOne, mustExclude);

            // Assert
            sut.MustInclude.Should().BeEquivalentTo(mustInclude);
            sut.AtLeastOne.Should().BeEquivalentTo(atLeastOne);
            sut.MustExclude.Should().BeEquivalentTo(mustExclude);
        }

        [Fact]
        public void Properties_WhenSet_ShouldHoldTheAssignedValues()
        {
            // Arrange
            var sut = new SearchQuery();
            var mustInclude = new List<string> { "new_term1" };
            var atLeastOne = new List<string> { "new_term2" };
            var mustExclude = new List<string> { "new_term3" };

            // Act
            sut.MustInclude = mustInclude;
            sut.AtLeastOne = atLeastOne;
            sut.MustExclude = mustExclude;

            // Assert
            sut.MustInclude.Should().BeSameAs(mustInclude);
            sut.AtLeastOne.Should().BeSameAs(atLeastOne);
            sut.MustExclude.Should().BeSameAs(mustExclude);
        }
    }
}