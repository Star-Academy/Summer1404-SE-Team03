using SearchEngine.Core.Model;

namespace SearchEngine.Core
{
    public class Searcher
    {
        private readonly InvertedIndexManager _indexManager;

        public Searcher(InvertedIndexManager indexManager)
        {
            _indexManager = indexManager;
        }

        public IEnumerable<string> SmartSearch(SearchQuery query)
        {
            var result = InitializeResultSet(query);
            result = FilterByAtLeastOne(result, query.AtLeastOne);
            result = FilterByMustExclude(result, query.MustExclude);
            return result;
        }

        private IEnumerable<string> InitializeResultSet(SearchQuery query)
        {
            if (query.MustInclude.Any())
                return PerformAndSearch(query.MustInclude);

            return _indexManager.GetAllDocuments();
        }


        private IEnumerable<string> PerformAndSearch(IEnumerable<string> mustInclude)
        {
            IEnumerable<string> result = null;
            foreach (var word in mustInclude)
            {
                var docs = _indexManager.GetDocumentsForToken(word.ToUpperInvariant());
                result = result == null ? docs : result.Intersect(docs);
            }
            return result ?? Enumerable.Empty<string>();
        }

        private IEnumerable<string> FilterByAtLeastOne(IEnumerable<string> currentResult, IEnumerable<string> atLeastOne)
        {
            if (!atLeastOne.Any())
            {
                return currentResult;
            }
            var orSet = atLeastOne.SelectMany(w => _indexManager.GetDocumentsForToken(w.ToUpperInvariant())).ToHashSet();
            return currentResult.Intersect(orSet);
        }

        private IEnumerable<string> FilterByMustExclude(IEnumerable<string> currentResult, IEnumerable<string> mustExclude)
        {
            if (!mustExclude.Any())
            {
                return currentResult;
            }
            var notSet = mustExclude.SelectMany(w => _indexManager.GetDocumentsForToken(w.ToUpperInvariant())).ToHashSet();
            return currentResult.Except(notSet);
        }
    }
}