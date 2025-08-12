using Xunit;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SearchEngine.Core.Model;

namespace SearchEngine.Tests
{
    public class SearchEngineTests : IDisposable
    {
        private readonly string _tempDirectoryPath;
        private readonly Dictionary<string, string> _fileContents;

        public SearchEngineTests()
        {
            _tempDirectoryPath = Path.Combine(Path.GetTempPath(), "SearchEngineTest_" + Guid.NewGuid());
            Directory.CreateDirectory(_tempDirectoryPath);

            _fileContents = new Dictionary<string, string>
            {
                { "doc1.txt", "The quick brown fox jumps over the lazy dog." },
                { "doc2.txt", "A quick C# search engine example." },
                { "doc3.txt", "Never forget the lazy dog." },
                { "doc4.txt", "Search and find anything you want." },
                { "doc5.txt", "C# is a powerful language, not a dog." }
            };

            foreach (var file in _fileContents)
            {
                File.WriteAllText(Path.Combine(_tempDirectoryPath, file.Key), file.Value);
            }
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectoryPath))
            {
                Directory.Delete(_tempDirectoryPath, true);
            }
        }

        public static IEnumerable<object[]> GetSearchTestData()
        {
            yield return new object[]
            {
                new SearchQuery(new[] { "search" }, new string[] { }, new string[] { }),
                new[] { "doc2.txt", "doc4.txt" }
            };

            yield return new object[]
            {
                new SearchQuery(new[] { "quick", "search" }, new string[] { }, new string[] { }),
                new[] { "doc2.txt" }
            };

            yield return new object[]
            {
                new SearchQuery(new string[] { }, new[] { "fox", "engine" }, new string[] { }),
                new[] { "doc1.txt", "doc2.txt" }
            };

            yield return new object[]
            {
                new SearchQuery(new string[] { }, new string[] { }, new[] { "lazy" }),
                new[] { "doc2.txt", "doc4.txt", "doc5.txt" }
            };

            yield return new object[]
            {
                new SearchQuery(new[] { "c" }, new string[] { }, new[] { "dog" }),
                new[] { "doc2.txt" }
            };

            yield return new object[]
            {
                new SearchQuery(new[] { "dog" }, new[] { "quick", "lazy" }, new string[] { }),
                new[] { "doc1.txt", "doc3.txt" }
            };

            yield return new object[]
            {
                new SearchQuery(new[] { "dog" }, new[] { "brown", "forget" }, new[] { "lazy" }),
                new string[] { }
            };

            yield return new object[]
            {
                new SearchQuery(new string[] { }, new string[] { }, new string[] { }),
                new[] { "doc1.txt", "doc2.txt", "doc3.txt", "doc4.txt", "doc5.txt" }
            };

            yield return new object[]
            {
                new SearchQuery(new[] { "nonexistent" }, new string[] { }, new string[] { }),
                new string[] { }
            };

            yield return new object[]
            {
                new SearchQuery(new[] { "search" }, new[] { "nonexistent" }, new string[] { }),
                new string[] { }
            };

            yield return new object[]
            {
                new SearchQuery(new[] { "search" }, new string[] { }, new[] { "example", "find" }),
                new string[] { }
            };

            yield return new object[]
            {
                new SearchQuery(new string[] { "search engine" }, new string[] {}, new string[] {}),
                new string[] { "doc2.txt" }
            };

            yield return new object[]
            {
                new SearchQuery(new string[] {}, new string[] { "powerful language" , "lazy dog"}, new string[] {}),
                new string[] { "doc1.txt", "doc3.txt", "doc5.txt"}
            };
            
            yield return new object[]
            {
                new SearchQuery(new string[] {}, new string[] { "powerful language" , "lazy dog"}, new[] {"quick brown"}),
                new string[] { "doc3.txt", "doc5.txt"}
            };
        }

        [Theory]
        [MemberData(nameof(GetSearchTestData))]
        public void Search_WithVariousQueries_ShouldReturnCorrectDocuments(SearchQuery query, IEnumerable<string> expectedFileNames)
        {
            var engine = new SearchEngine(_tempDirectoryPath);

            var result = engine.Search(query);
            var resultFileNames = result.Select(Path.GetFileName);

            Assert.NotNull(result);
            Assert.Equal(expectedFileNames.OrderBy(f => f), resultFileNames.OrderBy(f => f));
        }

        [Fact]
        public void Constructor_WithNonExistentDirectory_ShouldCreateIndexWithoutDocuments()
        {
            var nonExistentPath = Path.Combine(_tempDirectoryPath, "no_such_dir");
            var engine = new SearchEngine(nonExistentPath);

            var result = engine.Search(new SearchQuery(new string[] { }, new string[] { }, new string[] { }));
            
            Assert.Empty(result);
        }

        [Fact]
        public void Constructor_WithEmptyDirectory_ShouldCreateIndexWithoutDocuments()
        {
            var emptyDirPath = Path.Combine(_tempDirectoryPath, "empty_dir");
            Directory.CreateDirectory(emptyDirPath);
            var engine = new SearchEngine(emptyDirPath);

            var result = engine.Search(new SearchQuery(new string[] { }, new string[] { }, new string[] { }));

            Assert.Empty(result);
        }

        [Fact]
        public void Constructor_WithNullDirectoryPath_ShouldSucceedAndCreateEmptyIndex()
        {
            var engine = new SearchEngine(null);
            var result = engine.Search(new SearchQuery(new string[] { }, new string[] { }, new string[] { }));

            Assert.NotNull(engine);
            Assert.Empty(result);
        }
    }
}