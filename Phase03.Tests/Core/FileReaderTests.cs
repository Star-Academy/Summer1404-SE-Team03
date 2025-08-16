using FluentAssertions;
using SearchEngine.Core;
using Xunit;

namespace Phase03.Tests.Core
{
    public class FileReaderTests
    {
        [Fact]
        public void ReadAllFileNames_WhenFolderDoesNotExist_ShouldReturnEmptyArray()
        {
            // Arrange
            var nonExistentFolderPath = "nonexistent-folder";

            // Act
            var result = FileReader.ReadAllFileNames(nonExistentFolderPath);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void TryReadFile_WhenPathIsInvalid_ShouldReturnFalseAndNullContent()
        {
            // Arrange
            var invalidPath = "invalid.txt";

            // Act
            var success = FileReader.TryReadFile(invalidPath, out var content);

            // Assert
            success.Should().BeFalse();
            content.Should().BeNull();
        }
    }
}