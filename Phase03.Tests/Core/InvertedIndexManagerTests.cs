using Xunit;
using Moq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SearchEngine.Core.Processing;

namespace SearchEngine.Core.Tests
{
    public class InvertedIndexManagerTests : IDisposable
    {
        private readonly Mock<ITokenizer> _mockTokenizer;
        private readonly string _tempTestDirectory;

        public InvertedIndexManagerTests()
        {
            _mockTokenizer = new Mock<ITokenizer>();
            _tempTestDirectory = Path.Combine(Path.GetTempPath(), "InvertedIndexManagerTests_" + Guid.NewGuid());
            Directory.CreateDirectory(_tempTestDirectory);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempTestDirectory))
            {
                Directory.Delete(_tempTestDirectory, true);
            }
        }

        private string CreateTempFile(string fileName, string content)
        {
            var filePath = Path.Combine(_tempTestDirectory, fileName);
            File.WriteAllText(filePath, content);
            return filePath;
        }

        [Fact]
        public void Constructor_ShouldInitializeWithGivenTokenizer()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            Assert.NotNull(manager);
        }

        [Fact]
        public void AddTokenToIndex_NewToken_CreatesNewDocumentSet()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            var token = "TEST";
            var docId = "doc1";

            manager.AddTokenToIndex(token, docId);

            var documents = manager.GetDocumentsForToken(token);
            Assert.Single(documents, docId);
        }

        [Fact]
        public void AddTokenToIndex_ExistingToken_AddsDocumentToSet()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            var token = "TEST";
            var docId1 = "doc1";
            var docId2 = "doc2";

            manager.AddTokenToIndex(token, docId1);
            manager.AddTokenToIndex(token, docId2);

            var documents = manager.GetDocumentsForToken(token);
            Assert.Equal(2, documents.Count());
            Assert.Contains(docId1, documents);
            Assert.Contains(docId2, documents);
        }

        [Fact]
        public void AddTokenToIndex_DuplicateDocument_DoesNotAddDuplicate()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            var token = "TEST";
            var docId = "doc1";

            manager.AddTokenToIndex(token, docId);
            manager.AddTokenToIndex(token, docId);

            var documents = manager.GetDocumentsForToken(token);
            Assert.Single(documents, docId);
        }

        [Fact]
        public void GetDocumentsForToken_ExistingToken_ReturnsCorrectDocuments()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            var token = "EXISTING";
            var expectedDocs = new[] { "doc1", "doc3" };

            manager.AddTokenToIndex(token, expectedDocs[0]);
            manager.AddTokenToIndex(token, expectedDocs[1]);
            manager.AddTokenToIndex("OTHER_TOKEN", "doc2");

            var actualDocs = manager.GetDocumentsForToken(token);

            Assert.Equal(expectedDocs.Length, actualDocs.Count());
            Assert.All(expectedDocs, doc => Assert.Contains(doc, actualDocs));
        }

        [Fact]
        public void GetDocumentsForToken_NonExistentToken_ReturnsEmptyEnumerable()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            manager.AddTokenToIndex("SOME_TOKEN", "doc1");

            var documents = manager.GetDocumentsForToken("NON_EXISTENT_TOKEN");

            Assert.NotNull(documents);
            Assert.Empty(documents);
        }
        
        [Fact]
        public void GetAllDocuments_WhenIndexIsEmpty_ReturnsEmptyEnumerable()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            var allDocs = manager.GetAllDocuments();
            Assert.Empty(allDocs);
        }

        [Fact]
        public void GetAllDocuments_WhenIndexHasData_ReturnsAllDistinctDocuments()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            manager.AddTokenToIndex("TOKEN1", "doc1");
            manager.AddTokenToIndex("TOKEN1", "doc2");
            manager.AddTokenToIndex("TOKEN2", "doc2");
            manager.AddTokenToIndex("TOKEN3", "doc3");

            var allDocs = manager.GetAllDocuments().ToList();
            
            Assert.Equal(3, allDocs.Count);
            Assert.Contains("doc1", allDocs);
            Assert.Contains("doc2", allDocs);
            Assert.Contains("doc3", allDocs);
        }

        [Fact]
        public void AddDocument_WithValidFile_AddsTokensToIndex()
        {
            var fileContent = "Hello world";
            var filePath = CreateTempFile("test.txt", fileContent);
            var tokens = new[] { "HELLO", "WORLD" };
            _mockTokenizer.Setup(t => t.Tokenize(fileContent)).Returns(tokens);

            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            manager.AddDocument(filePath);

            Assert.Single(manager.GetDocumentsForToken("HELLO"), filePath);
            Assert.Single(manager.GetDocumentsForToken("WORLD"), filePath);
            _mockTokenizer.Verify(t => t.Tokenize(fileContent), Times.Once);
        }
        
        [Fact]
        public void AddDocument_WithNonExistentFile_DoesNothing()
        {
            var nonExistentFilePath = Path.Combine(_tempTestDirectory, "no_such_file.txt");
            var manager = new InvertedIndexManager(_mockTokenizer.Object);

            manager.AddDocument(nonExistentFilePath);

            Assert.Empty(manager.GetAllDocuments());
            _mockTokenizer.Verify(t => t.Tokenize(It.IsAny<string>()), Times.Never);
        }
    }
}