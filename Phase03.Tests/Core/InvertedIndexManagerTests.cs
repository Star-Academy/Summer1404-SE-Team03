using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using SearchEngine.Core;
using SearchEngine.Core.Interface;
using SearchEngine.Core.Model;
using Xunit;

namespace SearchEngine.Tests.Core
{
    public class InvertedIndexManagerTests : IDisposable
    {
        private readonly IInvertedIndexData _invertedIndexData;
        private readonly ITokenizer _tokenizer;
        private readonly INormalizer _normalizer;
        private readonly InvertedIndexManager _sut;
        private readonly string _tempDirectory;

        public InvertedIndexManagerTests()
        {
            _invertedIndexData = Substitute.For<IInvertedIndexData>();
            _tokenizer = Substitute.For<ITokenizer>();
            _normalizer = Substitute.For<INormalizer>();
            _sut = new InvertedIndexManager(_invertedIndexData);
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [Fact]
        public void Update_WhenGivenNewDataSource_ShouldChangeInternalDataSource()
        {
            // Arrange
            var initialData = Substitute.For<IInvertedIndexData>();
            var initialIndex = new Dictionary<string, HashSet<string>> { { "initial", new HashSet<string> { "doc1" } } };
            initialData.Index.Returns(initialIndex);
            var sut = new InvertedIndexManager(initialData);

            var newData = Substitute.For<IInvertedIndexData>();
            var newIndex = new Dictionary<string, HashSet<string>> { { "new", new HashSet<string> { "doc2" } } };
            newData.Index.Returns(newIndex);

            // Act
            sut.Update(newData);
            var result = sut.GetDocumentsForToken("new");
            var oldResult = sut.GetDocumentsForToken("initial");


            // Assert
            result.Should().Contain("doc2");
            result.Should().NotContain("doc1");
            oldResult.Should().BeEmpty();
        }

        [Fact]
        public void AddTokenToIndex_WhenTokenIsNew_ShouldAddNewTokenAndDocument()
        {
            // Arrange
            var index = new Dictionary<string, HashSet<string>>();
            _invertedIndexData.Index.Returns(index);
            _normalizer.Normalize("token").Returns("TOKEN");

            // Act
            _sut.AddTokenToIndex("token", "doc1", _normalizer);

            // Assert
            _normalizer.Received(1).Normalize("token");
            index.Should().ContainKey("TOKEN");
            index["TOKEN"].Should().Contain("doc1");
        }

        [Fact]
        public void AddTokenToIndex_WhenTokenExists_ShouldAddDocumentToExistingToken()
        {
            // Arrange
            var index = new Dictionary<string, HashSet<string>>
            {
                { "TOKEN", new HashSet<string> { "doc1" } }
            };
            _invertedIndexData.Index.Returns(index);
            _normalizer.Normalize("token").Returns("TOKEN");

            // Act
            _sut.AddTokenToIndex("token", "doc2", _normalizer);

            // Assert
            index["TOKEN"].Should().HaveCount(2);
            index["TOKEN"].Should().BeEquivalentTo(new[] { "doc1", "doc2" });
        }

        [Fact]
        public void GetDocumentsForToken_WhenTokenExists_ShouldReturnAssociatedDocuments()
        {
            // Arrange
            var expectedDocs = new HashSet<string> { "doc1", "doc2" };
            var index = new Dictionary<string, HashSet<string>>
            {
                { "TOKEN", expectedDocs }
            };
            _invertedIndexData.Index.Returns(index);

            // Act
            var result = _sut.GetDocumentsForToken("TOKEN");

            // Assert
            result.Should().BeEquivalentTo(expectedDocs);
        }

        [Fact]
        public void GetDocumentsForToken_WhenTokenDoesNotExist_ShouldReturnEmptyEnumerable()
        {
            // Arrange
            _invertedIndexData.Index.Returns(new Dictionary<string, HashSet<string>>());

            // Act
            var result = _sut.GetDocumentsForToken("NONEXISTENT");

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAllDocuments_WhenIndexHasDocuments_ShouldReturnAllDistinctDocuments()
        {
            // Arrange
            var index = new Dictionary<string, HashSet<string>>
            {
                { "TOKEN1", new HashSet<string> { "doc1", "doc2" } },
                { "TOKEN2", new HashSet<string> { "doc2", "doc3" } },
                { "TOKEN3", new HashSet<string> { "doc1", "doc3" } }
            };
            _invertedIndexData.Index.Returns(index);
            var expected = new List<string> { "doc1", "doc2", "doc3" };

            // Act
            var result = _sut.GetAllDocuments();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetAllDocuments_WhenIndexIsEmpty_ShouldReturnEmptyEnumerable()
        {
            // Arrange
            _invertedIndexData.Index.Returns(new Dictionary<string, HashSet<string>>());

            // Act
            var result = _sut.GetAllDocuments();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void AddDocument_WhenFileExists_ShouldAddAllItsTokensToIndex()
        {
            // Arrange
            var filePath = Path.Combine(_tempDirectory, "test.txt");
            var fileContent = "some content here";
            File.WriteAllText(filePath, fileContent);

            var normalizedContent = "SOME CONTENT HERE";
            var tokens = new[] { "SOME", "CONTENT", "HERE" };
            _normalizer.Normalize(fileContent).Returns(normalizedContent);
            _tokenizer.Tokenize(normalizedContent).Returns(tokens);

            var realIndexData = new InvertedIndexData();
            var sut = new InvertedIndexManager(realIndexData);

            // Act
            sut.AddDocument(filePath, _tokenizer, _normalizer);

            // Assert
            _normalizer.Received(1).Normalize(fileContent);
            _tokenizer.Received(1).Tokenize(normalizedContent);

            realIndexData.Index.Should().ContainKey("SOME").WhoseValue.Should().Contain(filePath);
            realIndexData.Index.Should().ContainKey("CONTENT").WhoseValue.Should().Contain(filePath);
            realIndexData.Index.Should().ContainKey("HERE").WhoseValue.Should().Contain(filePath);
        }

        [Fact]
        public void AddDocument_WhenFileDoesNotExist_ShouldDoNothing()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_tempDirectory, "nonexistent.txt");
            var realIndexData = new InvertedIndexData();
            var sut = new InvertedIndexManager(realIndexData);

            // Act
            sut.AddDocument(nonExistentPath, _tokenizer, _normalizer);

            // Assert
            _normalizer.DidNotReceive().Normalize(Arg.Any<string>());
            _tokenizer.DidNotReceive().Tokenize(Arg.Any<string>());
            realIndexData.Index.Should().BeEmpty();
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