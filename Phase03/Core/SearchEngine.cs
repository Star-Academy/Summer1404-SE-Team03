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
}