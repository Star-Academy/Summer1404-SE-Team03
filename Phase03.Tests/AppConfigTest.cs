using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace SearchEngine.Tests
{
    public class AppConfigTests
    {
        public AppConfigTests()
        {
        }

        [Fact]
        public void DataDirectory_WhenAccessed_ShouldReturnCorrectlyInitializedPath()
        {
            // Arrange
            var baseDir = AppContext.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            var expectedPath = Path.Combine(projectRoot, "EnglishData");

            // Act
            var actualPath = AppConfig.DataDirectory;

            // Assert
            actualPath.Should().NotBeNull();
            actualPath.Should().Be(expectedPath);
        }
    }
}