using System.IO;
using SearchEngine.Core;
using Xunit;

namespace Phase03.Tests.Core
{
    public class FileReaderTests
    {
        [Fact]
        public void ReadAllFileNames_NonExistingFolder_ReturnsEmptyArray()
        {
            var result = FileReader.ReadAllFileNames("nonexistent-folder");
            Assert.Empty(result);
        }

        [Fact]
        public void TryReadFile_InvalidPath_ReturnsFalseAndNullContent()
        {
            var success = FileReader.TryReadFile("invalid.txt", out var content);
            Assert.False(success);
            Assert.Null(content);
        }
    }
}
