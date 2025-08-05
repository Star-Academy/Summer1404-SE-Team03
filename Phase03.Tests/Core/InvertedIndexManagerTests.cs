using Xunit;
using Moq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SearchEngine.Core.Processing;
using SearchEngine.Core.Model;

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
        public void GetDocumentsForToken_IsCaseInsensitive_ReturnsCorrectDocuments()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            manager.AddTokenToIndex("HELLO", "doc1", 0);
            manager.AddTokenToIndex("WORLD", "doc1", 1);
            
            var documentsLower = manager.GetDocumentsForToken("hello world");
            var documentsUpper = manager.GetDocumentsForToken("HELLO WORLD");
            var documentsMixed = manager.GetDocumentsForToken("hELLo wORLd");

            Assert.Single(documentsLower, "doc1");
            Assert.Single(documentsUpper, "doc1");
            Assert.Single(documentsMixed, "doc1");
        }
        
        [Fact]
        public void AddTokenToIndex_NewToken_AddsTokenAndDocumentPosition()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            manager.AddTokenToIndex("TEST", "doc1", 0);
            var documents = manager.GetDocumentsForToken("test");
            Assert.Single(documents, "doc1");
        }

        [Fact]
        public void AddTokenToIndex_ExistingToken_AddsDocumentAndPosition()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            var token = "TEST";
            var docId1 = "doc1";
            var docId2 = "doc2";
            manager.AddTokenToIndex(token, docId1, 0);
            manager.AddTokenToIndex(token, docId2, 0);
            var documents = manager.GetDocumentsForToken("test");
            Assert.Equal(2, documents.Count());
            Assert.Contains(docId1, documents);
            Assert.Contains(docId2, documents);
        }
        
        [Fact]
        public void GetDocumentsForToken_ValidSequentialPhrase_ReturnsCorrectDocument()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            var docId = "doc1";
            manager.AddTokenToIndex("HELLO", docId, 0);
            manager.AddTokenToIndex("WORLD", docId, 1);
            manager.AddTokenToIndex("ANOTHER", "doc2", 0);
            manager.AddTokenToIndex("WORLD", "doc2", 1);

            var documents = manager.GetDocumentsForToken("hello world");

            Assert.Single(documents, docId);
        }

        [Fact]
        public void GetDocumentsForToken_NonSequentialPhrase_ReturnsEmpty()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            var docId = "doc1";
            manager.AddTokenToIndex("HELLO", docId, 0);
            manager.AddTokenToIndex("THERE", docId, 1);
            manager.AddTokenToIndex("WORLD", docId, 2);
            var documents = manager.GetDocumentsForToken("hello world");

            Assert.Empty(documents);
        }

        [Fact]
        public void AddDocument_Then_GetDocumentsForToken_PerformsCorrectIntegration()
        {
            var fileContent = "the quick brown fox jumps over the lazy dog and the fox is quick";
            var filePath = CreateTempFile("doc1.txt", fileContent);

            var tokensWithPositions = new List<(string, int)>
            {
                ("THE", 0), ("QUICK", 1), ("BROWN", 2), ("FOX", 3), ("JUMPS", 4),
                ("OVER", 5), ("THE", 6), ("LAZY", 7), ("DOG", 8), ("AND", 9),
                ("THE", 10), ("FOX", 11), ("IS", 12), ("QUICK", 13)
            };
            
            _mockTokenizer.Setup(t => t.TokenizeWithPositions(fileContent)).Returns(tokensWithPositions);
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            
            manager.AddDocument(filePath);
            
            var validPhraseResult1 = manager.GetDocumentsForToken("quick brown fox");
            Assert.Single(validPhraseResult1, filePath);
            
            var validPhraseResult2 = manager.GetDocumentsForToken("THE FOX IS QUICK");
            Assert.Single(validPhraseResult2, filePath);

            var invalidPhraseResult = manager.GetDocumentsForToken("the dog");
            Assert.Empty(invalidPhraseResult);
            
            var phraseWithMissingWord = manager.GetDocumentsForToken("quick brown cat");
            Assert.Empty(phraseWithMissingWord);
            
            _mockTokenizer.Verify(t => t.TokenizeWithPositions(fileContent), Times.Once);
        }

        [Fact]
        public void GetAllDocuments_WhenIndexHasData_ReturnsAllDistinctDocuments()
        {
            var manager = new InvertedIndexManager(_mockTokenizer.Object);
            manager.AddTokenToIndex("TOKEN1", "doc1", 0);
            manager.AddTokenToIndex("TOKEN1", "doc2", 0);
            manager.AddTokenToIndex("TOKEN2", "doc2", 1);
            manager.AddTokenToIndex("TOKEN3", "doc3", 0);

            var allDocs = manager.GetAllDocuments().ToList();
            
            Assert.Equal(3, allDocs.Count);
            Assert.Contains("doc1", allDocs);
            Assert.Contains("doc2", allDocs);
            Assert.Contains("doc3", allDocs);
        }
    }
}