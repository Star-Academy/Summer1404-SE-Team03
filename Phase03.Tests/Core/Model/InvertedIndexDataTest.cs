using Xunit;
using System.Collections.Generic;
using SearchEngine.Core.Model;

namespace SearchEngine.Tests.Core.Model
{
    public class InvertedIndexDataTests
    {
        [Fact]
        public void Constructor_ShouldInitializeIndex()
        {
            var invertedIndexData = new InvertedIndexData();

            Assert.NotNull(invertedIndexData.Index);
        }

        [Fact]
        public void Index_ShouldBeEmpty_AfterInitialization()
        {
            var invertedIndexData = new InvertedIndexData();

            Assert.Empty(invertedIndexData.Index);
        }
    }
}