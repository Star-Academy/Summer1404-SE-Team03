using System.IO;
using SearchEngine.Core;
using SearchEngine.Core.Processing;
using NSubstitute;
using Xunit;

namespace Phase03.Tests.Core
{
    public class InvertedIndexManagerTests
    {
        [Fact]
        public void AddTokenToIndex_NewToken_CreatesEntry()
        {
            var tokenizer = Substitute.For<ITokenizer>();
            var manager = new InvertedIndexManager(tokenizer);

            manager.AddTokenToIndex("test", "doc1");
            var docs = manager.GetDocumentsForToken("test");

            Assert.Contains("doc1", docs);
        }

        [Fact]
        public void AddDocument_FileExists_AddsAllTokens()
        {
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "hello world");

            var tokenizer = Substitute.For<ITokenizer>();
            tokenizer.Tokenize("hello world").Returns(new[] { "HELLO", "WORLD" });

            var manager = new InvertedIndexManager(tokenizer);

            manager.AddDocument(tempFile);

            Assert.Contains(tempFile, manager.GetDocumentsForToken("HELLO"));
            Assert.Contains(tempFile, manager.GetDocumentsForToken("WORLD"));

            File.Delete(tempFile);
        }
    }
}
