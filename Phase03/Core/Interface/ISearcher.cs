using SearchEngine.Core.Interface;

namespace SearchEngine.Core.Interface
{
    public interface ISearcher
    {
        public IEnumerable<string> SmartSearch(ISearchQuery query, IInvertedIndex indexManager);
    }
}