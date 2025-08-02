using Xunit;
using System;
using System.IO;
using SearchEngine;

namespace SearchEngine.Tests
{
    public class AppConfigTests
    {
        [Fact]
        public void DataDirectory_ShouldBeCorrectlyInitialized()
        {
            var baseDir = AppContext.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            var expectedPath = Path.Combine(projectRoot, "EnglishData");

            var actualPath = AppConfig.DataDirectory;

            Assert.NotNull(actualPath);
            Assert.EndsWith(Path.DirectorySeparatorChar + "EnglishData", actualPath);
            Assert.Equal(expectedPath, actualPath);
        }
    }
}