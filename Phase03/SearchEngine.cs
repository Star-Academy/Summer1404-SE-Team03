using SearchEngine.Core;
using SearchEngine.Core.Model;
using SearchEngine.Core.Processing;
using SearchEngine.UI;

namespace SearchEngine
{
    public class SearchEngine
    {
        private readonly Searcher searcher;

        public SearchEngine(string dataDir)
        {
            var normalizer = new Normalizer();
            var tokenizer = new Tokenizer(normalizer);
            var index = new InvertedIndexManager(tokenizer);

            var files = FileReader.ReadAllFileNames(dataDir);
            foreach (var file in files)
            {
                index.AddDocument(file);
            }

            searcher = new Searcher(index);
        }

        public IEnumerable<string> Search(SearchQuery query)
        {
            return searcher.SmartSearch(query);
        }
    }

    public class Program{
        public static void Main(string[] args)
        {
            var dataDir = AppConfig.DataDirectory;            
            var searchEngine = new SearchEngine(dataDir);
            
            var consoleUi = new ConsoleUi();
            var query = consoleUi.GetQueryFromUser();
            var results = searchEngine.Search(query);
            consoleUi.DisplayResults(results);
        }
    }
}