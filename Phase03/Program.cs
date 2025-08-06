using System.Diagnostics.CodeAnalysis;
using SearchEngine.Core;
using SearchEngine.UI;
using SearchEngine.Core.Model;
using SearchEngine.Core.Processing;

namespace SearchEngine
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            var dataDir = AppConfig.DataDirectory;

            var normalizer = new Normalizer();
            var tokenizer = new Tokenizer();
            var index = new InvertedIndexManager(new InvertedIndexData());

            var files = FileReader.ReadAllFileNames(dataDir);
            foreach (var file in files)
            {
                index.AddDocument(file, tokenizer, normalizer);
            }

            var searcher = new Searcher();
            var searchEngine = new SearchEngine();

            var consoleUi = new ConsoleUi();
            var query = consoleUi.GetQueryFromUser<SearchQuery>(normalizer);
            var results = searchEngine.Search(query,searcher,index);
            consoleUi.DisplayResults(results);
        }
    }
}