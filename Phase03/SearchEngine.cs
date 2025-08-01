using SearchEngine.Core;
using SearchEngine.Core.Model;
using SearchEngine.Core.Processing;
using SearchEngine.UI;

namespace SearchEngine
{
    public class SearchEngine
    {
        private readonly InvertedIndex _index;

        public SearchEngine(InvertedIndex index)
        {
            _index = index;
        }

        public void BuildIndex(string dataDir)
        {
            var files = FileReader.ReadAllFileNames(dataDir);
            foreach (var file in files)
            {
                _index.AddDocument(file);
            }
        }

        public IEnumerable<string> Search(SearchQuery query)
        {
            return _index.SmartSearch(query);
        }

        public static void Main(string[] args)
        {
            var dataDir = AppConfig.DataDirectory;

            var normalizer = new Normalizer();
            var tokenizer = new Tokenizer(normalizer);
            var index = new InvertedIndex(tokenizer);
            var searchEngine = new SearchEngine(index);

            searchEngine.BuildIndex(dataDir);

            var consoleUi = new ConsoleUi();
            var query = consoleUi.GetQueryFromUser();
            var results = searchEngine.Search(query);
            consoleUi.DisplayResults(results);
        }
    }
}