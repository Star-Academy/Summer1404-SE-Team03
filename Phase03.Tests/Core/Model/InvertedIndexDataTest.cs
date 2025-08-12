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
            var token = "hello";
            var docId = "doc1";
            var position = 5;

            var positionsSet = new HashSet<int> { position };
            var postingsList = new Dictionary<string, HashSet<int>>
            {
                [docId] = positionsSet
            };

            invertedIndexData.Index[token] = postingsList;
            
            Assert.Single(invertedIndexData.Index);
            Assert.True(invertedIndexData.Index.ContainsKey(token));
            Assert.Equal(postingsList, invertedIndexData.Index[token]);

            Assert.True(invertedIndexData.Index[token].ContainsKey(docId));
            Assert.Single(invertedIndexData.Index[token][docId]);
            Assert.Contains(position, invertedIndexData.Index[token][docId]);
        }

        [Fact]
        public void Index_CanAddPositionToExistingDocumentForToken()
        {
            var invertedIndexData = new InvertedIndexData();
            var token = "world";
            var docId = "doc1";
            
            invertedIndexData.Index[token] = new Dictionary<string, HashSet<int>>
            {
                [docId] = new HashSet<int> { 0 }
            };
            
            Assert.Single(invertedIndexData.Index[token][docId]);
            
            var newPosition = 10;
            invertedIndexData.Index[token][docId].Add(newPosition);
            
            Assert.Equal(2, invertedIndexData.Index[token][docId].Count);
            Assert.Contains(0, invertedIndexData.Index[token][docId]);
            Assert.Contains(newPosition, invertedIndexData.Index[token][docId]);
        }

        [Fact]
        public void Index_CanAddDocumentToExistingToken()
        {
            var invertedIndexData = new InvertedIndexData();
            var token = "test";
            var docId1 = "doc1";
            
            invertedIndexData.Index[token] = new Dictionary<string, HashSet<int>>
            {
                [docId1] = new HashSet<int> { 3 }
            };
            
            Assert.Single(invertedIndexData.Index[token]);

            var docId2 = "doc2";
            var newPositions = new HashSet<int> { 0, 8 };
            invertedIndexData.Index[token][docId2] = newPositions;

            Assert.Equal(2, invertedIndexData.Index[token].Count);
            
            Assert.Contains(docId1, invertedIndexData.Index[token].Keys);
            Assert.Contains(docId2, invertedIndexData.Index[token].Keys);
            
            Assert.Equal(newPositions, invertedIndexData.Index[token][docId2]);
        }
    }
}