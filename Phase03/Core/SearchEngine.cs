using SearchEngine.Core;
using SearchEngine.Core.Interface;
using SearchEngine.Core.Model;
using SearchEngine.Core.Processing;
using SearchEngine.UI;

namespace SearchEngine
{
    public class SearchEngine
    {
        public IEnumerable<string> Search(ISearchQuery query, ISearcher _searcher, IInvertedIndex _invertedindex)
        {
            return _searcher.SmartSearch(query, _invertedindex);
        }
    }
}