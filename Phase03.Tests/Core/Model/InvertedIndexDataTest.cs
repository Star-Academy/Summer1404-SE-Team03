using Xunit;
using System.Collections.Generic;
using SearchEngine.Core.Model;

namespace SearchEngine.Core.Model.Tests
{
    public class InvertedIndexDataTests
    {
        [Fact]
        public void Constructor_ShouldInitializeNonNullAndEmptyIndex()
        {
            var invertedIndexData = new InvertedIndexData();

            Assert.NotNull(invertedIndexData.Index);
            Assert.Empty(invertedIndexData.Index);
        }

        [Fact]
        public void Index_CanBePopulatedAfterInitialization()
        {
            var invertedIndexData = new InvertedIndexData();
            var word = "hello";
            var documentId = "doc1";
            var documentSet = new HashSet<string> { documentId };

            invertedIndexData.Index[word] = documentSet;
            
            Assert.Single(invertedIndexData.Index);
            Assert.True(invertedIndexData.Index.ContainsKey(word));
            Assert.Equal(documentSet, invertedIndexData.Index[word]);
            Assert.Contains(documentId, invertedIndexData.Index[word]);
        }

        [Fact]
        public void Index_ExistingHashSetCanBeModified()
        {
            var invertedIndexData = new InvertedIndexData();
            var word = "world";
            var documentId1 = "doc1";
            var documentId2 = "doc2";
            
            invertedIndexData.Index[word] = new HashSet<string> { documentId1 };
            
            Assert.Single(invertedIndexData.Index[word]);
            
            invertedIndexData.Index[word].Add(documentId2);
            
            Assert.Equal(2, invertedIndexData.Index[word].Count);
            Assert.Contains(documentId1, invertedIndexData.Index[word]);
            Assert.Contains(documentId2, invertedIndexData.Index[word]);
        }
    }
}