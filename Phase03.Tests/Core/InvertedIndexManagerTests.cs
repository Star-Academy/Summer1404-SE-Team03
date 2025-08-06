using Xunit;
using Moq;
using SearchEngine.Core.Interface;
using SearchEngine.Core;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using SearchEngine.Core.Model;

namespace SearchEngine.Tests.Core
{
    public class InvertedIndexManagerTests : IDisposable
    {
        private readonly Mock<IInvertedIndexData> _mockInvertedIndexData;
        private readonly Mock<ITokenizer> _mockTokenizer;
        private readonly Mock<INormalizer> _mockNormalizer;
        private readonly InvertedIndexManager _invertedIndexManager;
        private readonly string _tempDirectory;

        public InvertedIndexManagerTests()
        {
            _mockInvertedIndexData = new Mock<IInvertedIndexData>();
            _mockTokenizer = new Mock<ITokenizer>();
            _mockNormalizer = new Mock<INormalizer>();
            _invertedIndexManager = new InvertedIndexManager(_mockInvertedIndexData.Object);
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [Fact]
        public void Update_ShouldChangeInternalDataSource()
        {
            var initialData = new Mock<IInvertedIndexData>();
            var initialIndex = new Dictionary<string, HashSet<string>> { { "initial", new HashSet<string> { "doc1" } } };
            initialData.Setup(d => d.Index).Returns(initialIndex);
            var manager = new InvertedIndexManager(initialData.Object);

            var newData = new Mock<IInvertedIndexData>();
            var newIndex = new Dictionary<string, HashSet<string>> { { "new", new HashSet<string> { "doc2" } } };
            newData.Setup(d => d.Index).Returns(newIndex);

            manager.Update(newData.Object);
            var result = manager.GetDocumentsForToken("new");

            Assert.Contains("doc2", result);
            Assert.DoesNotContain("doc1", result);
            Assert.Empty(manager.GetDocumentsForToken("initial"));
        }

        [Fact]
        public void AddTokenToIndex_WithNormalizer_AddsNewToken()
        {
            var index = new Dictionary<string, HashSet<string>>();
            _mockInvertedIndexData.Setup(d => d.Index).Returns(index);
            _mockNormalizer.Setup(n => n.Normalize(It.IsAny<string>())).Returns<string>(s => s.ToUpper());

            _invertedIndexManager.AddTokenToIndex("token", "doc1", _mockNormalizer.Object);

            Assert.True(index.ContainsKey("TOKEN"));
            Assert.Contains("doc1", index["TOKEN"]);
            _mockNormalizer.Verify(n => n.Normalize("token"), Times.Once);
        }

        [Fact]
        public void AddTokenToIndex_WithNormalizer_AddsToExistingToken()
        {
            var index = new Dictionary<string, HashSet<string>>
            {
                { "TOKEN", new HashSet<string> { "doc1" } }
            };
            _mockInvertedIndexData.Setup(d => d.Index).Returns(index);
            _mockNormalizer.Setup(n => n.Normalize(It.IsAny<string>())).Returns<string>(s => s.ToUpper());

            _invertedIndexManager.AddTokenToIndex("token", "doc2", _mockNormalizer.Object);

            Assert.Equal(2, index["TOKEN"].Count);
            Assert.Contains("doc1", index["TOKEN"]);
            Assert.Contains("doc2", index["TOKEN"]);
        }

        [Fact]
        public void GetDocumentsForToken_TokenExists_ReturnsDocuments()
        {
            var expectedDocs = new HashSet<string> { "doc1", "doc2" };
            var index = new Dictionary<string, HashSet<string>>
            {
                { "TOKEN", expectedDocs }
            };
            _mockInvertedIndexData.Setup(d => d.Index).Returns(index);

            var result = _invertedIndexManager.GetDocumentsForToken("TOKEN");

            Assert.Equal(expectedDocs, result);
        }

        [Fact]
        public void GetDocumentsForToken_TokenDoesNotExist_ReturnsEmptyEnumerable()
        {
            var index = new Dictionary<string, HashSet<string>>();
            _mockInvertedIndexData.Setup(d => d.Index).Returns(index);

            var result = _invertedIndexManager.GetDocumentsForToken("NONEXISTENT");

            Assert.Empty(result);
        }

        [Fact]
        public void GetAllDocuments_ShouldReturnDistinctDocuments()
        {
            var index = new Dictionary<string, HashSet<string>>
            {
                { "TOKEN1", new HashSet<string> { "doc1", "doc2" } },
                { "TOKEN2", new HashSet<string> { "doc2", "doc3" } },
                { "TOKEN3", new HashSet<string> { "doc1", "doc3" } }
            };
            _mockInvertedIndexData.Setup(d => d.Index).Returns(index);
            var expected = new List<string> { "doc1", "doc2", "doc3" };

            var result = _invertedIndexManager.GetAllDocuments().ToList();

            Assert.Equal(expected.Count, result.Count);
            Assert.All(expected, item => Assert.Contains(item, result));
        }

        [Fact]
        public void GetAllDocuments_EmptyIndex_ReturnsEmptyEnumerable()
        {
            var index = new Dictionary<string, HashSet<string>>();
            _mockInvertedIndexData.Setup(d => d.Index).Returns(index);

            var result = _invertedIndexManager.GetAllDocuments();

            Assert.Empty(result);
        }

        [Fact]
        public void AddDocument_FileExists_AddsTokensToIndex()
        {
            var filePath = Path.Combine(_tempDirectory, "test.txt");
            var fileContent = "some content here";
            File.WriteAllText(filePath, fileContent);

            var normalizedContent = "SOME CONTENT HERE";
            var tokens = new[] { "SOME", "CONTENT", "HERE" };

            var realIndexData = new InvertedIndexData();
            var manager = new InvertedIndexManager(realIndexData);

            _mockNormalizer.Setup(n => n.Normalize(fileContent)).Returns(normalizedContent);
            _mockTokenizer.Setup(t => t.Tokenize(normalizedContent)).Returns(tokens);

            manager.AddDocument(filePath, _mockTokenizer.Object, _mockNormalizer.Object);

            _mockNormalizer.Verify(n => n.Normalize(fileContent), Times.Once);
            _mockTokenizer.Verify(t => t.Tokenize(normalizedContent), Times.Once);

            Assert.True(realIndexData.Index.ContainsKey("SOME"));
            Assert.Contains(filePath, realIndexData.Index["SOME"]);
            Assert.True(realIndexData.Index.ContainsKey("CONTENT"));
            Assert.Contains(filePath, realIndexData.Index["CONTENT"]);
            Assert.True(realIndexData.Index.ContainsKey("HERE"));
            Assert.Contains(filePath, realIndexData.Index["HERE"]);
        }

        [Fact]
        public void AddDocument_FileDoesNotExist_DoesNothing()
        {
            var nonExistentPath = Path.Combine(_tempDirectory, "nonexistent.txt");
            var realIndexData = new InvertedIndexData();
            var manager = new InvertedIndexManager(realIndexData);

            manager.AddDocument(nonExistentPath, _mockTokenizer.Object, _mockNormalizer.Object);

            _mockNormalizer.Verify(n => n.Normalize(It.IsAny<string>()), Times.Never);
            _mockTokenizer.Verify(t => t.Tokenize(It.IsAny<string>()), Times.Never);
            Assert.Empty(realIndexData.Index);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }
    }
}